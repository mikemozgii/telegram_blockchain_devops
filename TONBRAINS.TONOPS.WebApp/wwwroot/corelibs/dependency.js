window.grpcServiceCache = {};

async function dependency(name, cacheAlias) {

    const defaultBinding = getDependencyDefaultBinding();

    if (name === `componentStyles`) return globalComponentStyles;

    if (name.indexOf('globalMixins/') === 0) return globalMixins[name.substring(13)];

    if (name.indexOf(`grpcservice/`) === 0) return window.grpcServiceCache[name.replace(`grpcservice/`, ``)];

    if (`additionalDependenciesMixins` in window) {
        const { isHandled, result } = window.additionalDependenciesMixins(name);
        if (isHandled) return result;
    }

    if (name.indexOf(`calendarMixin/`) === 0) return calendarMixin[name.substring(14)];

    if (name in defaultBinding) return defaultBinding[name];

    if (!window.dependencyHash) window.dependencyHash = {};

    if (name in window.dependencyHash) {
        return window.dependencyHash[name];
    }

    const {data: content} = await axios.get(name);
    
    const module = { exports: {} };
    try {
        const cacheName = cacheAlias ? cacheAlias : name;
        Function("exports", "require", "module", content).call(module.exports, module.exports, dependency, module);   
        window.dependencyHash[cacheName] = module.exports;
        return window.dependencyHash[cacheName];
    } catch (e) {
        //TODO: Syntax error handler!!!!!
        console.warn(`Error within dependency ${name} at: ` + e);
    }
}

function getDependencyFromCache(name){
    return name in window.dependencyHash ? window.dependencyHash[name] : null;
}

async function putProsemirror() {
    await dependency(`${window.env.cs}corelibs/tiptap/orderedmap.js`, "orderedmap");

    await dependency(`${window.env.cs}corelibs/tiptap/prosemirror-model.js`, "prosemirror-model");
    await dependency(`${window.env.cs}corelibs/tiptap/prosemirror-transform.js`, "prosemirror-transform");
    await dependency(`${window.env.cs}corelibs/tiptap/prosemirror-state.js`, "prosemirror-state");
    await dependency(`${window.env.cs}corelibs/tiptap/prosemirror-view.js`, "prosemirror-view");
    await dependency(`${window.env.cs}corelibs/tiptap/w3c-keyname.js`, "w3c-keyname");
    await dependency(`${window.env.cs}corelibs/tiptap/keymap.js`, "prosemirror-keymap");

    await dependency(`${window.env.cs}corelibs/tiptap/dropcursor.js`, "prosemirror-dropcursor");
    await dependency(`${window.env.cs}corelibs/tiptap/gapcursor.js`, "prosemirror-gapcursor");
    await dependency(`${window.env.cs}corelibs/tiptap/prosemirror-commands.js`, "prosemirror-commands");
    await dependency(`${window.env.cs}corelibs/tiptap/prosemirror-inputrules.js`, "prosemirror-inputrules");
    await dependency(`${window.env.cs}corelibs/tiptap/tables.js`, "prosemirror-tables");

    await dependency(`${window.env.cs}corelibs/tiptap/prosemirror-utils.js`, "prosemirror-utils");
    await dependency(`${window.env.cs}corelibs/tiptap/tiptap-utils.js`, "tiptap-utils");

    await dependency(`${window.env.cs}corelibs/tiptap/prosemirror-schema-list.js`, "prosemirror-schema-list");
    await dependency(`${window.env.cs}corelibs/tiptap/tiptap-commands.js`, "tiptap-commands");
    await dependency(`${window.env.cs}corelibs/tiptap/tiptap.js`, "tiptap");

    await dependency(`${window.env.cs}corelibs/tiptap/rope-sequence.js`, "rope-sequence");
    await dependency(`${window.env.cs}corelibs/tiptap/history.js`, "prosemirror-history");

    await dependency(`${window.env.cs}corelibs/tiptap/highlight.js`, "highlight");
    await dependency(`${window.env.cs}corelibs/tiptap/format-min.js`, "format");
    await dependency(`${window.env.cs}corelibs/tiptap/fault.js`, "fault");
    await dependency(`${window.env.cs}corelibs/tiptap/lowlight.js`, "lowlight");
    await dependency(`${window.env.cs}corelibs/tiptap/extensions.js`, "tiptap-extensions");
}

function getDependencyDefaultBinding() {
    return {
        'axios': axios,
        'jscookie': Cookies,
        'pdfjs': PDFJS,
        'moment': moment,
        'moment-range': momentRange,
        'pdfjs-viewer': PDFViewerApplication,
        'chart': Chart,
        'environment': window.env
        //'cropper': Cropper,
        /*'html2canvas': html2canvas,
        'jspdf': jsPDF*/
    }
}

const componentDependencyHash = {};

const batchingComponentDependency = {};

function getComponentHash(path) {
    let hash = 0
    let chr;
    for (let i = 0; i < path.length; i++) {
        chr = path.charCodeAt(i);
        hash = ((hash << 5) - hash) + chr;
        hash |= 0;
    }
    return hash.toString();
}

async function getComponentContentFromServer(path) {
    let { data: result } =  await axios.get(path);

    if (result.indexOf(`<vue-plain-template>`) > -1) result = result.replace(/\<vue\-plain\-template\>/, "<template>\n<div>");
    if (result.indexOf(`</vue-plain-template>`) > -1) result = result.replace(/\<\/vue\-plain\-template\>/, "</div>\n</template>");
    if (result.indexOf(`<vue-template>`) > -1) result = result.replace(/\<vue\-template\>/, "<template>\n<div class='container-component-id'>");
    if (result.indexOf(`</vue-template>`) > -1) result = result.replace(/\<\/vue\-template\>/, "</div>\n</template>");

    //create hash for component
    if (result.indexOf(`-component-id`) > -1) {
        const componentId = getComponentHash(path);
        return result.replace(/\-component\-id/g, componentId);
    }

    return result;
}

async function getComponentContent(path) {
    if (path in batchingComponentDependency) {
        return await batchingComponentDependency[path];
    }

    batchingComponentDependency[path] = getComponentContentFromServer(path);
    const content = await batchingComponentDependency[path];

    delete batchingComponentDependency[path];

    return content;
}

function isCommonComponentPath(path) {
    const lowerPath = path.toLowerCase();
    if (lowerPath.indexOf(window.env.cs) === 0) return true; //common cheching folders

    if (lowerPath.indexOf(`.cached`) > -1) return true; //user defined caching files and folders

    return false;
}

async function componentDependency(path) {
    const isCommonComponent = isCommonComponentPath(path);
    if (path in componentDependencyHash && isCommonComponent) return Promise.resolve(componentDependencyHash[path]);

    const content = await getComponentContent(path);

    if (isCommonComponent) componentDependencyHash[path] = content;

    return content;
}

function clearComponentsCache() {
    for (const key in Object.keys(componentDependencyHash)) {
        delete componentDependencyHash[key];
    }
}