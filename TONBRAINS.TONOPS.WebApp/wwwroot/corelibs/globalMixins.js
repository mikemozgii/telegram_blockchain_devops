const lastTabRefreshKey = `lastabrefresh`;

const lastTabRefresh = {
    time: localStorage.getItem(lastTabRefreshKey)
};

let orgStructure = {};

if (!lastTabRefresh.time) {
    lastTabRefresh.time = new Date().getTime();
    localStorage.setItem(lastTabRefreshKey, lastTabRefresh.time);
} else {
    lastTabRefresh.time = parseInt(lastTabRefresh.time);
}

const modalCounter = {
    countModels: 0
};

setInterval(() => {
    const value = localStorage.getItem(lastTabRefreshKey);
    if (!value) return;

    if (lastTabRefresh.time !== parseInt(value)) location.reload();
}, 2000); // may be need more second??????????

const globalObserver = {};

function registerListener(event, handler) {
    if (event in globalObserver) {
        globalObserver[event].push(handler);
    } else {
        globalObserver[event] = [handler];
    }
}

function unregisterListener(event, handler) {
    if (event in globalObserver) globalObserver[event] = globalObserver[event].filter(a => a !== handler);
}

function fireEvent(event, parameters) {
    if (!(event in globalObserver)) return;

    const handlers = globalObserver[event];

    for (const handler of handlers) handler(parameters);
}

const globalTranslationsData = {
    calendar: {
        en: {
            "weekDays": ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"],
            "weekDaysShort": ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"],
            "weekDaysExtraShort": ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"],
            "months": ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
            "years": "Years",
            "year": "Year",
            "month": "Month",
            "week": "Week",
            "day": "Day",
            "today": "Today",
            "noEvent": "No Event",
            "allDay": "All day",
            "deleteEvent": "Delete",
            "createEvent": "Create an event",
            "dateFormat": "DDDD mmmm d{S}, yyyy",
            "dateWatermark": "mm/dd/yyyy",
            "clear": "Clear"
        },
        fr: {
            "weekDays": ["Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi", "Samedi", "Dimanche"],
            "months": ["Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre"],
            "years": "Années",
            "year": "Année",
            "month": "Mois",
            "week": "Semaine",
            "day": "Jour",
            "today": "Aujourd'hui",
            "noEvent": "Aucun événement",
            "allDay": "Jour entier",
            "deleteEvent": "Supprimer",
            "createEvent": "Créer un événement",
            "dateFormat": "DDDD d mmmm yyyy",
            "weekDaysExtraShort": ["Di", "Lu", "Ma", "Me", "Je", "Ve", "Sa"],
            "dateWatermark": "mm/jj/aaaa",
            "clear": "Effacer"
        }
    }
};

let globalTranslationsLanguage = "en";

const usedLotusServices = {};

function getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

function formatWebUrl(serviceUrl) {
    if (serviceUrl[serviceUrl.length - 1] === "/") serviceUrl = serviceUrl.substring(0, serviceUrl.length - 1);

    return serviceUrl;
}

function getServiceUrl(service) {
    const environmentModules = window.env.lotusEnvironment.modules.filter(a => a.name === service);

    if (environmentModules.length === 1) return formatWebUrl(environmentModules[0].webUrl);

    const randomIndex = getRandomInt(0, environmentModules.length - 1);

    return formatWebUrl(environmentModules[randomIndex].webUrl);
}

function getUsedService(service) {
    if (service in usedLotusServices) {
        return usedLotusServices[service];
    }
    const serviceUrl = getServiceUrl(service);
    usedLotusServices[service] = serviceUrl;

    return serviceUrl;
}

function checkModulesLoaded() {
    return new Promise((resolve, reject) => {
        if ('lotusEnvironment' in window.env && window.env.lotusEnvironment.modules) {
            resolve(true);
            return;
        }

        setTimeout(
            () => {
                resolve(true);
            },
        1000);
    });
}

const cachedTranslations = {};

const globalMixins = {
    grpc: {
        methods: {
            async getConfigService() {
                const serviceUrl = getUsedService(`config`);

                await dependency(`${serviceUrl}/grpcwebservice.js`);
                const service = await dependency(`grpcservice/config`);

                service.client = new service.ConfigurationSchemeClient(serviceUrl);

                return service;
            },
            async getHrService() {
                const serviceUrl = getUsedService(`hr`);

                await dependency(`${serviceUrl}/grpcwebservice.js`);
                const service = await dependency(`grpcservice/hr`);

                service.client = new service.HumanResourcesServiceClient(serviceUrl);

                return service;
            },
            async getHsService() {
                const serviceUrl = getUsedService(`hs`);

                await dependency(`${serviceUrl}/grpcwebservice.js`);
                const service = await dependency(`grpcservice/hs`);

                service.client = new service.HealthAndSafetyServiceClient(serviceUrl);

                return service;
            },
            getGrpcFieldName(originalName) {
                const firstCharacter = originalName.charAt(0).toUpperCase();
                return firstCharacter + originalName.substring(1);
            },
            setGrpcModel(target, model) {
                for (const key of Object.keys(model)) {
                    let setMethod = `set${this.getGrpcFieldName(key)}`;
                    if (typeof (model[key]) === `object` && model[key].length) setMethod += `List`;
                    if (!target[setMethod]) continue;
                    target[setMethod](model[key]);
                }
                return target;
            },
            executeGrpcMethod(method, client, request) {
                return new Promise(
                    (resolve, reject) => {
                        const runMethod = client[method].bind(client);
                        const cookie = document.cookie;
                        const index = cookie.indexOf(`dtoken=`);
                        let token = ``;
                        if (index !== -1) {
                            token = cookie.substring(index + `dtoken=`.length, );
                            const separatorIndex = token.indexOf(`;`);
                            if (separatorIndex > -1) token = token.substring(0, separatorIndex);
                        }
                        runMethod(request, { 'token': decodeURIComponent(token) }, (err, response) => {
                            if (err) {
                                reject(new Error(`Grpc error: method(${ method }) code(${ err.code }) ${ err.message }`));
                            } else {
                                resolve(response);
                            }
                        });
                    }
                );
            }
        }
    },
    orgTree: {
        methods: {
            orgTreeConditionPromise(module) {
                return new Promise(function (resolve, reject) {
                    setTimeout(function () {
                        reject();
                    }, 5000);
                    function loop() {
                        if (orgStructure[module] && orgStructure[module].done) {
                            return resolve();
                        }
                        setTimeout(loop, 0);
                    }
                    setTimeout(loop, 0);
                });
            },
            async getTree(module) {

                if (orgStructure[module] && Date.now() - orgStructure[module].timestamp < 10000) {
                    await this.orgTreeConditionPromise(module);
                    return orgStructure[module].data;
                }

                orgStructure[module] = {
                    data: [],
                    timestamp: Date.now(),
                    done: false
                };
                const environment = window.env.lotusEnvironment.environment;
                const configService = await globalMixins.grpc.methods.getConfigService();
                const structureResult = await globalMixins.grpc.methods.executeGrpcMethod(
                    `organizationTreeData`,
                    configService.client,
                    globalMixins.grpc.methods.setGrpcModel(new configService.OrganizationTreeDataRequest(), { environment: environment, module })
                );
                const result = structureResult.getItemsList().map(i => i.toObject());

                orgStructure[module] = {
                    data: result,
                    timestamp: Date.now(),
                    done: true
                };

                return result;
            }
        }
    },
    translations: {
        methods: {
            getTranslation(group, key, options) {
                const language = globalTranslationsLanguage;
                const cacheKey = group + key + language;

                if (cacheKey in cachedTranslations) return cachedTranslations[cacheKey];

                if (!(group in globalTranslationsData)) {
                    console.warn(`Translation group ${group} don't exists!`);
                    return "";
                }
                if (!(language in globalTranslationsData[group])) {
                    console.warn(`Translation language ${key} in group ${group} don't exists!`);
                    return "";
                }
                if (!(key in globalTranslationsData[group][language])) {
                    console.info(`"${key}": "${key}",`);
                    return key;
                }

                const translateValue = globalTranslationsData[group][language][key];
                if ({}.toString.call(translateValue) === `[object Function]`) return translateValue(options);

                cachedTranslations[cacheKey] = translateValue;

                return translateValue;
            },
            changeLanguage(language) {
                globalTranslationsLanguage = language;
                this.$forceUpdate();
                for (const component of this.$children) {
                    component.$forceUpdate();
                }
                //TODO: change language in all childrens
            }
        }
    },
    modalsCounter: {
        methods: {
            incrementCounter() {
                modalCounter.countModels++;
            },
            releaseCounter() {
                if (modalCounter.countModels > 0) modalCounter.countModels--;
            },
            counterIsEmpty() {
                return modalCounter.countModels === 0;
            }
        }
    },
    mobileDetector: {
        methods: {
            openOnMobileDevice() {
                const userAgent = navigator.userAgent.toLowerCase();
                return userAgent.indexOf("android") > -1 || userAgent.indexOf("iphone") > -1;
            }
        }
    },
    tonHelper: {
        methods: {
            fromNg(item) {
                var r = Number(item) * 0.000000001
                return r;
            },
            toNg(item) {
                var r = Number(item) * 1000000000
                return r;
            },
        }
    },
    globalObserver: {
        methods: {
            register(event, handler) {
                registerListener(event, handler);
            },
            unregister(event, handler) {
                unregisterListener(event, handler);
            },
            fireEvent(event, parameters) {
                fireEvent(event, parameters);
            }
        }
    },
    validate: {
        props: {
            validate: {
                type: Array,
                default: _ => []
            },
            validatecontext: {
                type: Object,
                default: _ => { }
            }
        },
        data() {
            return {
                validateErrors: [],
                validateHash: this.generateValidateHash()
            }
        },
        created() {
            this.refreshValidate();
        },
        beforeDestroy() {
            if (!this.validatecontext || !this.validateHash) return;

            delete this.validatecontext[this.validateHash];
        },
        methods: {
            validateMessage(rule, defaultMessage) {
                const message = rule.message && rule.message instanceof Function ? rule.message() : null;
                const errorMessage = message || rule.message || defaultMessage;
                this.validateErrors.push(errorMessage);
            },
            validateValue() {
                if (!this.validate || !this.validate.length) {
                    if (!this.validatecontext)
                        return;
                    var res = this.validatecontext[this.validateHash];
                    if (!res) return;
                    res.isValid = true;
                    this.validateErrors = [];
                    this.validatecontext.checkValidate();
                    this.$emit(`validate-changed`, res.isValid);
                    return;
                }

                const validateResult = this.validatecontext[this.validateHash];

                if (!validateResult) return; //WORKAROUND: because autocomplete in browser work before initializing validate context :(

                validateResult.isValid = true;
                this.validateErrors = [];

                for (const rule of this.validate) {
                    let ruleName = rule.name || rule;
                    if (this.defaultValidateRules && ruleName in this.defaultValidateRules) {
                        if (!this.defaultValidateRules[ruleName](rule)) {
                            validateResult.isValid = false;
                        }
                        continue;
                    }
                    if (rule.handler) {
                        if (!rule.handler(rule, this.validateMessage)) {
                            validateResult.isValid = false;
                        }
                    }
                }

                //check all form validate
                this.validatecontext.checkValidate();
                this.$emit(`validate-changed`, validateResult.isValid);
            },
            generateValidateHash() {
                return Math.random().toString(36).substring(2);
            },
            refreshValidate() {
                if (!this.validatecontext) return;

                if (this.validate) {
                    this.$set(
                        this.validatecontext,
                        this.validateHash,
                        {
                            isValid: false,
                            forceValidate: this.validateValue
                        }
                    );
                } else {
                    this.validateErrors = [];
                    if (this.validatecontext && this.validateHash in this.validatecontext) this.validatecontext[this.validateHash] = {};
                }
            }
        }
    },
    validateHost: {
        data() {
            return {
                validateResult: {
                    checkValidate: this.checkValidate
                },
                validateSummary: false,
                requiredOnlyRule: ['required']
            }
        },
        methods: {
            checkValidate() {
                const values = Object.values(this.validateResult).filter(a => a !== this.checkValidate);
                this.validateSummary = !values.find(a => !a.isValid);
            },
            forceValidate() {
                Object.values(this.validateResult).filter(a => a !== this.checkValidate).forEach(a => a.forceValidate());
            },
            validateReset() {
                this.validateResult = {
                    checkValidate: this.checkValidate
                };
                this.validateSummary = false;
            }
        },
        computed: {
            isValid() {
                this.forceValidate();
                this.checkValidate();

                return this.validateSummary;
            }
        }
    },
    clickAway: {
        data() {
            return {
                clickawayName: null,
                clickawayCheck: false,
                clickawayHtmlElement: null
            }
        },
        beforeDestroy() {
            document.documentElement.removeEventListener('click', this.clickawayHtmlElement[this.clickawayName], false);
            delete this.clickawayHtmlElement[this.clickawayName];
        },
        methods: {
            generateClickAwayComponentHash() {
                return Math.random().toString();
            },
            forceClickAway() {
                document.documentElement.click();
            },
            forceClickAwayWithoutItself() {
                this.clickawayCheck = true;
                this.forceClickAway();
            },
            setupClickAway(name, callback, element) {
                const htmlElement = element ? element : this.$el;
                this.clickawayHtmlElement = htmlElement;
                this.clickawayName = name;
                htmlElement[name] = (ev) => {
                    if (!htmlElement.contains(ev.target) && !this.clickawayCheck) {
                        callback();
                    }
                    this.clickawayCheck = false;
                }
                document.documentElement.addEventListener('click', htmlElement[name], false);
            }

        }
    },
    fileInfoHelper: {
        data() {
            return {
                imageFileTypes: [`jpg`, `jpeg`, `png`, `bmp`, `ico`, `gif`],
                fileTypeColors: {
                    'folder': `orange`,
                    'jpg': `mediumseagreen`,
                    'jpeg': `mediumseagreen`,
                    'png': `mediumseagreen`,
                    'bmp': `mediumseagreen`,
                    'ico': `mediumseagreen`,
                    'gif': `mediumseagreen`,
                    'pdf': `red`,
                    'doc': `dodgerblue`,
                    'rtf': `dodgerblue`,
                    'docx': `dodgerblue`
                },
                fileServiceUrl: getUsedService(`filestorage`)
            }
        },
        methods: {
            getToken() {
                const cookie = document.cookie;
                const index = cookie.indexOf(`dtoken=`);
                let token = ``;
                if (index !== -1) {
                    token = cookie.substring(index + `dtoken=`.length,);
                    const separatorIndex = token.indexOf(`;`);
                    if (separatorIndex > -1) token = token.substring(0, separatorIndex);
                }
                return token;
            },
            async getFilesInfo(ids, environment) {
                if (!ids || ids.length === 0) return [];

                const model = {
                    ids: ids,
                    token: decodeURIComponent(this.getToken()),
                    environment: environment
                }
                const { data: filesInfo } = await axios.post(`${this.fileServiceUrl}/api/files`, model);
                return filesInfo;
            },
            async deleteFiles(ids, environment) {
                if (!ids || ids.length === 0) return;

                const model = {
                    ids: ids,
                    token: decodeURIComponent(this.getToken()),
                    environment: environment
                }
                await axios.post(`${this.fileServiceUrl}/api/files/delete`, model);
            },
            async uploadFile(file, environment) {
                const formData = new FormData();
                formData.append(`content`, file);
                formData.append(`token`, decodeURIComponent(this.getToken()));
                formData.append(`environment`, environment);
                const options = {
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    }
                }

                const result = await axios.post(`${this.fileServiceUrl}/api/files/upload`, formData, options).catch((e) => null);
                return result ? result.data : null;
            },
            calculateSize(bytes, decimals) {
                if (bytes == 0) return '';
                var k = 1024,
                    dm = decimals <= 0 ? 0 : decimals || 2,
                    sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'],
                    i = Math.floor(Math.log(bytes) / Math.log(k));
                return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
            },
            downloadFile(fileId, environment) {
                if (!fileId) return;
                window.location.href = `${this.fileServiceUrl}/api/files/download?fileId=${fileId}&environment=${environment}`;
            },
            downloadZipedFiles(zipName, fileIds) {
                if (zipName && fileIds && fileIds.length > 0) {
                    let ulrDownloadFilesInZip = "";
                    fileIds.forEach(fileId => {
                        ulrDownloadFilesInZip += "&fileIds=" + fileId;
                    })
                    window.open(`/api/v3/Images/DownloadFilesInZip?&zipName=${zipName}${ulrDownloadFilesInZip}`, `_blank`);
                }
            },
            getIconByFileName(fileName) {
                const splited = fileName.split(`.`);
                const extension = splited && splited.length > 1 ? `.${splited[splited.length - 1]}` : ``;
                return this.getIcon(extension);
            },
            getIcon(extension) {
                if (!extension) return 'fas fa-file';
                switch (extension.toLowerCase()) {
                    case '.pdf': {
                        return 'far fa-file-pdf';
                    }
                    case '.png':
                    case '.jpg':
                    case '.jpeg':
                    case '.bmp':
                        return 'far fa-file-image';
                    default:
                        return 'fas fa-file';
                }
            },
            getIconColorByFileName(fileName) {
                const splited = fileName.split(`.`);
                const extension = splited && splited.length > 1 ? `.${splited[splited.length - 1]}` : ``;
                return this.getIconColor(extension);
            },
            getIconColor(fileExtension) {
                const extension = fileExtension.replace(`.`, ``).toLowerCase().trim();
                const color = this.fileTypeColors[extension];
                return color || 'dodgerblue';
            },
            getDialogType(fileExtension) {
                if (!fileExtension) return null;
                const extension = fileExtension.replace(`.`, ``).toLowerCase().trim();
                if (extension === `pdf`) return `pdf`;
                if (extension === `doc` || extension === `docx`) return `word`;
                if (this.imageFileTypes.indexOf(extension) >= 0) return `image`;
                return null;
            }
        }
    },
    viewDocumentModal: {
        data() {
            return {
                pdfDialog: null,
                imageView: null               
            }
        },
        methods: {
            setDialogs(pdfDialog, imageView) {               
                this.pdfDialog = pdfDialog;
                this.imageView = imageView;
            },
            viewDialog(item, id) {                   
                const dialogType = this.getDialogType(item.extension);
                if (!dialogType) {
                    this.downloadFile(id);
                    return;
                }
                switch (dialogType) {
                    case `pdf`: {
                        this.viewPdf(item, id);
                        break;
                    }
                    case `image`: {
                        this.viewImage(item, id);
                        break;
                    }
                    case `word`: {
                        this.viewWord(item, id);
                        break;
                    }
                    default: throw Error(`Not supported type.`);
                }
            },
            viewImage(item, id) {
                if (!this.imageView) return;

                this.imageView.show(id, item.fileName);
            },
            viewPdf(item, id) {
                if (!this.pdfDialog) return;

                this.pdfDialog.show(id, item.name);
            },
            viewWord(item, id) {
                
                if (!this.pdfDialog) return;

                this.pdfDialog.show(id, item.name, `/api/v3/Images/DocFileAsPdf?fileId=${id}`, null, true);
            }
        }
    },
    attachments: {
        methods: {
            async attachFilesToEntity(entityId, filesId, entityTypeId) {
                try {
                    await axios.post(
                        `/api/v3/images/attachtoentity`,
                        {
                            entityId: entityId,
                            filesId: filesId,
                            entityTypeId: entityTypeId
                        }
                    );
                    return true;
                } catch (e) {
                    return false;
                }
            },
            async getFilesForEntity(entityId) {
                try {
                    const { data: result } = await axios.get(
                        `/api/v3/images/entityfiles`,
                        {
                            params: {
                                entityId: entityId
                            }
                        }
                    );
                    return result;
                } catch (e) {
                    return [];
                }
            },
            async getFilesForEntityFull(entityId) {
                try {
                    const { data: result } = await axios.get(
                        `/api/v3/images/entityfilesfull`,
                        {
                            params: {
                                entityId: entityId
                            }
                        }
                    );
                    return result;
                } catch (e) {
                    return [];
                }
            }
        }
    },
    busy: {
        data() {
            return {
                isBusy: false,
                isPageSpinner: false
            }
        },
        methods: {
            setBusy() {

            },
            unsetBusy() {

            },
            setBusyState(state) {
                if (state) {
                    this.setBusy();
                } else {
                    this.unsetBusy();
                }
            },
        },
        computed: {
            busyVisible() {
                return this.isBusy ? 'visible' : 'collapsed';
            }
        }
    },
    tabs: {
        data() {
            return {
                tabsContext: {
                    tab: null,
                    items: [],
                    visited: {}
                }
            };
        },
        methods: {
            tabsSaveTab(alias, id) {
                localStorage.setItem(alias, id);
            },
            tabsRestoreTab(alias, defaultValue) {
                const id = localStorage.getItem(alias);
                return !id ? defaultValue : id;
            },
            tabsAddTabItem(componentId, title, security, slot) {
                if (!security) return;

                const tabItem = {
                    title: title,
                    id: componentId,
                    slot
                };
                this.tabsContext.items.push(tabItem);
                this.tabsContext.visited[componentId] = false;
                if (!this.tabsContext.tab) {
                    this.tabsContext.tab = componentId;
                    this.tabsContext.visited[componentId] = true;
                }
            },
            tabsChangeTab(id) {
                this.tabsContext.tab = id;
                this.tabsContext.visited[id] = true;
            },
        }
    },
    tabsRefresher: {
        methods: {
            refreshAllTabsInBrowser() {
                lastTabRefresh.time = new Date().getTime();
                localStorage.setItem(lastTabRefreshKey, lastTabRefresh.time);
            }
        }
    },
    gridFiltering: {
        data() {
            return {
                filterData: {
                    filters: [],
                    sortings: []
                }
            }
        },
        methods: {
            equalFilter(item, field) {
                const filter = this.filterData.filters.find(a => a.field === field);
                if (!filter) return true;
                switch (filter.type) {
                    case 1: //text fields - equal operator it underlying contains operator
                        return item[field] ? item[field].indexOf(filter.value) > -1 : false;
                    case 2:
                        return this.datesEquals(item[field], filter.value);
                    case 7:
                        {
                            if (!filter.value) {
                                return !item[field];
                            }
                            return item[field];
                        }
                    case 8: {
                        return this.employeeIntersects(item[field], filter.employeeIds);
                    }
                    default:
                        return item[field] === filter.value;
                }
            },
            employeeIntersects(fieldVal, filterVal) {
                if (Array.isArray(fieldVal)) {
                    return fieldVal.filter(value => -1 !== filterVal.indexOf(value));
                } else {
                    return filterVal.indexOf(fieldVal) > -1;
                }
            },
            isNotEualFilter(item, field) {
                const filter = this.filterData.filters.find(a => a.field === field);
                if (!filter) return true;
                return item[field] !== filter.value;
            },
            containsFilter(item, field) {
                const filter = this.filterData.filters.find(a => a.field === field);
                if (!filter) return true;
                return item[field] ? item[field].indexOf(filter.value) > -1 : false;
            },
            datesEquals(fieldVal, filterVal) {
                const format = `L`;
                let date = moment(fieldVal).format(format);
                return date === filterVal;
            },
            isLessThanFilter(item, field) {
                const filter = this.filterData.filters.find(a => a.field === field);
                if (!filter) return true;
                return item[field] < filter.value;
            },
            isLessThanOrEqualToFilter(item, field) {
                const filter = this.filterData.filters.find(a => a.field === field);
                if (!filter) return true;
                return item[field] <= filter.value;
            },
            isGreaterThanFilter(item, field) {
                const filter = this.filterData.filters.find(a => a.field === field);
                if (!filter) return true;
                return item[field] > filter.value;
            },
            isGreaterThanOrEqualToFilter(item, field) {
                const filter = this.filterData.filters.find(a => a.field === field);
                if (!filter) return true;
                return item[field] >= filter.value;
            },
            doesNotContanFilter(item, field) {
                const filter = this.filterData.filters.find(a => a.field === field);
                if (!filter) return true;
                return item[field] ? item[field].indexOf(filter.value) === -1 : false;
            },
            beginWithFilter(item, field) {
                const filter = this.filterData.filters.find(a => a.field === field);
                if (!filter) return true;
                return item[field] ? item[field].startsWith(filter.value) === true : false;
            },
            endWithFilter(item, field) {
                const filter = this.filterData.filters.find(a => a.field === field);
                if (!filter) return true;
                return item[field] ? item[field].endsWith(filter.value) === true : false;
            },
            isNullFilter(item, field) {
                const filter = this.filterData.filters.find(a => a.field === field);
                if (!filter) return true;
                return item[field] ? item[field] == null : false;
            },
            isNotNullFilter(item, field) {
                const filter = this.filterData.filters.find(a => a.field === field);
                if (!filter) return true;
                return item[field] ? item[field] != null : false;
            },
            isEmptyFilter(item, field) {
                const filter = this.filterData.filters.find(a => a.field === field);
                if (!filter) return true;
                return item[field] ? item[field] === '' : false;
            },
            isNotEmptyFilter(item, field) {
                const filter = this.filterData.filters.find(a => a.field === field);
                if (!filter) return true;
                return item[field] ? item[field] !== '' : false;
            },
            isBetweenFilter(item, field) {
                const filter = this.filterData.filters.find(a => a.field === field);
                if (!filter) return true;
                switch (filter.type) {                   
                    case 2: {
                        const format = `L`;
                        let date = moment(item[field]).format(format);
                        return date >= filter.value && date <= filter.toValue;
                    }
                    default:
                        return item[field] >= filter.value && item[field] <= filter.toValue;
                }               
            },
            isNotBetweenFilter(item, field) {
                const filter = this.filterData.filters.find(a => a.field === field);
                if (!filter) return true;
                switch (filter.type) {
                    case 2: {
                        const format = `L`;
                        let date = moment(item[field]).format(format);
                        return date < filter.value || date > filter.toValue;
                    }
                    default:
                        return item[field] < filter.value || item[field] > filter.toValue;
                }  
            },
            getFilterHandler(filterOption) {
                switch (filterOption.operator) {
                    case 1:
                        return this.equalFilter;
                    case 2:
                        return this.containsFilter;
                    case 3:
                        return this.isNotEualFilter;
                    case 4:
                        return this.isLessThanFilter;
                    case 5:
                        return this.isLessThanOrEqualToFilter;
                    case 6:
                        return this.isGreaterThanFilter;
                    case 7: 
                        return this.isGreaterThanOrEqualToFilter;
                    case 8:
                        return this.doesNotContanFilter;
                    case 9:
                        return this.beginWithFilter;
                    case 10 :
                        return this.endWithFilter;
                    case 11:
                        return this.isNullFilter;
                    case 12: 
                        return this.isNotNullFilter;
                    case 13:
                        return this.isEmptyFilter;
                    case 14:
                        return this.isNotEmptyFilter;
                    case 15:
                        return this.isBetweenFilter;
                    case 16:
                        return this.isNotBetweenFilter;

                    default: return null;
                }
            },
            getFilterValue(name) {
                if (!this.filterData) {
                    return null;
                }
                let filter = this.filterData.filters.find(f => f.field === name);
                return filter ? filter.value : null;
            }

        }
    },
    gridHelpers: {
        data() {
            return {
                gridData: [],
                parentPropertyName: null,
                identifierPropertyName: null,
                ignoreCaseSensitive: false,
                localStrategyReloadUrl: null,
                localStrategyReloadFunc: null
            }
        },
        methods: {
            generateId() {
                var PUSH_CHARS = '-0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz';
                var lastPushTime = 0;
                var lastRandChars = [];

                return function () {
                    var now = new Date().getTime();
                    var duplicateTime = (now === lastPushTime);
                    lastPushTime = now;

                    var timeStampChars = new Array(8);
                    for (var i = 7; i >= 0; i--) {
                        timeStampChars[i] = PUSH_CHARS.charAt(now % 64);
                        now = Math.floor(now / 64);
                    }
                    if (now !== 0) throw new Error('We should have converted the entire timestamp.');

                    var id = timeStampChars.join('');

                    if (!duplicateTime) {
                        for (i = 0; i < 12; i++) {
                            lastRandChars[i] = Math.floor(Math.random() * 64);
                        }
                    } else {
                        for (i = 11; i >= 0 && lastRandChars[i] === 63; i--) {
                            lastRandChars[i] = 0;
                        }
                        lastRandChars[i]++;
                    }
                    for (i = 0; i < 12; i++) {
                        id += PUSH_CHARS.charAt(lastRandChars[i]);
                    }
                    if (id.length != 20) throw new Error('Length should be 20.');

                    return id;
                }();
            },
            gridDateFormat(date, format = 'MM/DD/YYYY', inputFormat = 'YYYY-MM-DD') {
                return date ? moment(date, inputFormat).format(format) : '';
            },
            grpcDateFormat(unixTime, format = 'MM/DD/YYYY') {
                return unixTime ? moment((unixTime - 621355968000000000) / 10000).format(format) : '';
            },
            grpcDateTicks(date, format = 'MM/DD/YYYY') {
                return date ? (moment(date, format).toDate().getTime() * 10000) + 621355968000000000 : 0;
            },
            grpcNow() {
                return (Date.now() * 10000) + 621355968000000000;
            },
            async downloadExcelReport(apiUrl, model, name){
                await this.downloadFileReport(apiUrl, model, name, 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
            },
            async downloadCsvReport(apiUrl, model, name){
                await this.downloadFileReport(apiUrl, model, name, 'application/octet-stream');
            },
            async downloadFileReport(apiUrl, model, name, type){
                const options = { 
                    headers: [
                        {'Content-Type': 'application/json'}
                    ],
                    responseType: 'arraybuffer'
                };

                const { data: data } = await axios.post(apiUrl, model, options);

                const blob = new Blob([data], { type });
                const url = URL.createObjectURL(blob);
                const anchor = document.createElement('a');
                anchor.setAttribute('href', url);
                anchor.setAttribute('download', name);
                anchor.click();
                URL.revokeObjectURL(url);
                anchor.remove();
            },
            editItemInGrid(item, key) {
                const itemNeedUpdate = this.gridData.find(a => a[key] === item[key]);
                if (!itemNeedUpdate) return;

                Object.keys(item).forEach(a => {
                    itemNeedUpdate[a] = item[a];
                });
            },
            addItemToGrid(item) {
                item.expandable = false;
                item.expand = false;
                item.hidden = false;

                if (item[this.parentPropertyName]) {
                    const parent = this.gridData.find(a => a[this.identifierPropertyName] === item[this.parentPropertyName]);
                    item.treeHash = parent.treeHash + `.${item[this.identifierPropertyName]}`;
                } else {
                    item.treeHash = item[this.identifierPropertyName];
                }
                this.gridData.push(item);
            },
            deleteItemFromGrid(item) {
                if (!this.gridData) return;
                this.gridData = this.gridData.filter(a => !(a.treeHash === item.treeHash || a.treeHash.indexOf(item.treeHash) === 0));
            },
            fillTreeFields(items) {
                const resultData = [];
                for (const item of items) {
                    item.expandable = true;
                    item.expand = true;
                    item.hidden = false;

                    //WORKAROUND: Hi guys, you don't need see this ballshit, sorry for it :(
                    resultData.push(JSON.parse(JSON.stringify(item)))
                }
                return resultData;
            },
            getLocalTreeStrategy(parentProperty, identifierProperty) {
                if (!parentProperty) throw new Error('ParentProperty not specified! It is mandatory field.');

                this.parentPropertyName = parentProperty;
                this.identifierPropertyName = identifierProperty;

                const getHash = (parentId) => {
                    const parentNode = this.gridData.find(a => a[identifierProperty] === parentId);
                    if (!parentNode) return ``;

                    return `${(parentNode[parentProperty] ? getHash(parentNode[parentProperty]) + `.` : ``)}${parentId.toString()}`;
                };

                // items with null's need to be first of all
                this.gridData.sort((left, right) => {
                    if (!left[parentProperty]) return -1;
                    if (!right[parentProperty]) return 1;
                    return 0;
                });

                for (const item of this.gridData) {
                    if (!item[parentProperty]) {
                        item.treeHash = item[identifierProperty].toString();
                    } else {
                        item.treeHash = `${getHash(item[parentProperty])}.${item[identifierProperty]}`;
                    }
                }

                const sortFunction = function (left, right, isDesc) {
                    if (left === right) return 0;
                    return left > right ? (isDesc ? -1 : 1) : (isDesc ? 1 : -1);
                }

                const sortFunctionIgnoreCaseSensitive = function (left, right, isDesc) {
                    if (left === right) return 0;
                    return left.toString().toLowerCase() > right.toString().toLowerCase() ? (isDesc ? -1 : 1) : (isDesc ? 1 : -1);
                }

                const sortTree = (sortingDescending, nameField, gridData, rightFields) => {
                    if (!gridData.length) return;
                    const sort = this.ignoreCaseSensitive ? sortFunctionIgnoreCaseSensitive : sortFunction;

                    let sortByFieldDescFunc = this.sortByFieldDesc;
                    let sortByFieldAscFunc = this.sortByFieldAsc;
                    gridData.sort(function (left, right) {
                        if (left.treeHash.split(`.`).length === right.treeHash.split(`.`).length) {
                            const innerSort = sort(left[nameField], right[nameField], sortingDescending);
                            if (innerSort === 0) {
                                if (rightFields && rightFields.length) {
                                    const sortFunc = rightFields[0].descending
                                        ? sortByFieldDescFunc
                                        : sortByFieldAscFunc;
                                    return sortFunc(rightFields[0].field, rightFields.slice(1))(left, right);
                                }
                                return 0;
                            } else {
                                return innerSort;
                            }
                        }

                        return sort(left.treeHash.split(`.`).length, right.treeHash.split(`.`).length, false);
                    });

                }

                const getAllChildrensInRoot = (root, visibleData) => {
                    let result = [];
                    const childrens = visibleData.filter(
                        a => a.treeHash.split(`.`).length > root.treeHash.split(`.`).length &&
                            a.treeHash.indexOf(root.treeHash) === 0 &&
                            a.treeHash.indexOf(`.`, root.treeHash.length) === a.treeHash.lastIndexOf(`.`)
                    );
                    if (childrens.length === 0) {
                        root.expandable = false;
                        return [];
                    } else {
                        root.expandable = true;
                    }

                    for (const children of childrens) {
                        result = result.concat([children], getAllChildrensInRoot(children, visibleData));
                    }

                    return result;
                }

                const getGroupingTreeAsFlatList = (visibleData) => {
                    var root = this.gridData.find((item) => { return !item[parentProperty]; });

                    if (!root)
                        return [];

                    const rootHashLength = root.treeHash.split(`.`).length;
                    const roots = visibleData.filter(a => a.treeHash.split(`.`).length === rootHashLength);

                    let result = [];
                    for (const root of roots) {
                        result = result.concat([root], getAllChildrensInRoot(root, visibleData));
                    }

                    return result;
                }

                const fillMissedParents = (filteredData, parentId) => {
                    if (!parentId) return;
                    const parent = this.gridData.find(a => a[this.identifierPropertyName] === parentId);
                    if (!filteredData.find(a => a === parent)) {
                        filteredData.push(parent);
                    }

                    return fillMissedParents(filteredData, parent[this.parentPropertyName]);
                }

                const filteringData = (pageData) => {
                    const filteredData = this.gridData.filter(a => {
                        const keys = Object.keys(pageData.filters);

                        return keys.every(b => {
                            const condition = pageData.filters[b];
                            if (condition && condition.apply) return condition(a, b);
                            return a[b] === condition;
                        });
                    });

                    for (const item of filteredData) fillMissedParents(filteredData, item[this.parentPropertyName]);

                    return filteredData;
                }

                const checkRootParent = (item, pagingParents, collection) => {
                    const parent = collection.find(a => a[this.identifierPropertyName] === item[this.parentPropertyName]);
                    if (!parent[this.parentPropertyName]) return pagingParents.find(a => a[this.identifierPropertyName] === parent[this.identifierPropertyName]);

                    return checkRootParent(parent, pagingParents, collection);
                }

                const loadPage = (pageData) => {
                    const startIndex = (pageData.page - 1) * pageData.pageSize;

                    if (startIndex >= this.gridData.length) {
                        return {
                            count: this.gridData.length,
                            rows: []
                        }
                    }

                    let visibleData = this.gridData;

                    if (pageData.filters && Object.keys(pageData.filters).length) {
                        visibleData = filteringData(pageData);
                    }

                    if (pageData.sortingColumns && pageData.sortingColumns.length) {
                        const sortingColumn = pageData.sortingColumns[0];
                        const sortingField = sortingColumn.field;
                        const nameField = sortingField.charAt(0).toLowerCase() + sortingField.slice(1);
                        const rightColumns = pageData.sortingColumns.slice(1);
                        var newRightColumns = [];
                        rightColumns.forEach(item => newRightColumns.push({ field: item.field.charAt(0).toLowerCase() + item.field.slice(1), descending: item.descending }));
                        sortTree(sortingColumn.descending, nameField, visibleData, newRightColumns);
                    } else {
                        visibleData.sort(function (left, right) {
                            return sortFunction(left, right, false);
                        });
                    }

                    const allData = getGroupingTreeAsFlatList(visibleData);

                    const parents = allData.filter(a => !a[this.parentPropertyName]);
                    const pagingParents = parents.slice(startIndex, startIndex + pageData.pageSize);

                    const rows = [];
                    const parentIdName = this.parentPropertyName;
                    const idName = this.identifierPropertyName;

                    for (const item of allData) {
                        if (item[parentIdName]) {
                            const onCurentPage = checkRootParent(item, pagingParents, allData);
                            if (onCurentPage) rows.push(item);
                        } else {
                            const onCurentPage = pagingParents.find(a => a[idName] === item[idName]);
                            if (onCurentPage) rows.push(item);
                        }
                    }

                    return {
                        count: parents.length,
                        rows: rows
                    }
                }

                return {
                    loadPage
                }
            },
            sortByFieldAsc(field, rightFields) {
                return (left, right) => {
                    let leftValue = !left[field] ? `` : left[field];
                    let rightValue = !right[field] ? `` : right[field];
                    if ((typeof leftValue) === `string` && (typeof rightValue) === `string`) {
                        leftValue = leftValue.toLowerCase();
                        rightValue = rightValue.toLowerCase();
                    }
                    if (leftValue === rightValue) {
                        if (rightFields && rightFields.length) {
                            const sortFunc = rightFields[0].descending ? this.sortByFieldDesc : this.sortByFieldAsc;
                            return sortFunc(rightFields[0].field, rightFields.slice(1))(left, right);
                        }
                        return 0;
                    }
                    return leftValue < rightValue ? -1 : 1;
                };
            },
            sortByFieldDesc(field, rightFields) {
                return (left, right) => {
                    let leftValue = !left[field] ? `` : left[field];
                    let rightValue = !right[field] ? `` : right[field];
                    if ((typeof leftValue) === `string` && (typeof rightValue) === `string`) {
                        leftValue = leftValue.toLowerCase();
                        rightValue = rightValue.toLowerCase();
                    }
                    if (leftValue === rightValue) {
                        if (rightFields && rightFields.length) {
                            const sortFunc = rightFields[0].descending ? this.sortByFieldDesc : this.sortByFieldAsc;
                            return sortFunc(rightFields[0].field, rightFields.slice(1))(left, right);
                        }
                        return 0;
                    }
                    return leftValue > rightValue ? -1 : 1;
                };
            },
            getLocalStrategy(isNeedReloadData, reloadUrl, reloadFunc) {
                const filteringData = (pageData) => {
                    const filteredData = this.gridData.filter(a => {
                        const keys = Object.keys(pageData.filters);

                        return keys.every(b => {
                            const condition = pageData.filters[b];
                            if (condition.apply) return condition(a, b);
                            return a[b] === condition;
                        });
                    });

                    return filteredData;
                }

                const filteringFullTextData = (pageData) => {
                    const filteredData = this.gridData.filter(a => {
                        const keys = Object.keys(a);
                        return keys.find(b => {
                            if (a[b] === null) return false; //WORKAROUND: JS magic

                            return `${a[b]}`.toLowerCase().indexOf(pageData.filter.toLowerCase()) > -1;
                        });
                    });

                    return filteredData;
                }

                const loadPage = (pageData) => {
                    if (reloadUrl && !this.localStrategyReloadUrl) this.localStrategyReloadUrl = reloadUrl;
                    if (reloadFunc && !this.localStrategyReloadFunc) this.localStrategyReloadFunc = reloadFunc;

                    const startIndex = (pageData.page - 1) * pageData.pageSize;

                    if (startIndex >= this.gridData.length) {
                        return {
                            count: this.gridData.length,
                            rows: []
                        }
                    }

                    let visibleData = this.gridData;
                    if (pageData.filters && Object.keys(pageData.filters).length) visibleData = filteringData(pageData);
                    if (pageData.filter) visibleData = filteringFullTextData(pageData);
                    if (pageData.page == 1 && pageData.sortingColumns && pageData.sortingColumns.length) {
                        const sortingColumns = pageData.sortingColumns.map(a =>{
                            const field = a.field;
                            a.field = field.charAt(0).toLowerCase() + field.slice(1)
                            return a;
                        });
                        const firstSorting = sortingColumns[0];
                        const nameField = firstSorting.field;
                        visibleData.sort(firstSorting.descending ? this.sortByFieldDesc(nameField, sortingColumns.slice(1)) : this.sortByFieldAsc(nameField, sortingColumns.slice(1)));
                    }

                    return {
                        count: visibleData.length,
                        rows: visibleData.slice(startIndex, startIndex + pageData.pageSize)
                    }
                }

                const addItem = (item) => {
                    const gridData = this.gridData;
                    gridData.push(item);
                    this.gridData = gridData;
                }

                const deleteItem = (item) => {
                    const items = this.gridData.filter(a => a !== item);
                    this.gridData = items;
                }

                const reloadData = async () => {
                    if (this.localStrategyReloadFunc) {
                        this.gridData = await this.localStrategyReloadFunc();
                        return;
                    }
                    if (this.localStrategyReloadUrl) {
                        const { data: result } = await axios.get(this.localStrategyReloadUrl);
                        this.gridData = result;
                    }
                }

                return {
                    isNeedReloadData,
                    reloadData,
                    loadPage,
                    addItem,
                    deleteItem
                }
            },
            addColumn(title, field, { width, columnPoints, slot, notSortable, minWidthVisible, formatter, notVisible, isTreeColumn, headerSlot, notFilterable, type, filterHandler, filterSlot } = {}) {
                if (isTreeColumn) slot = `tree_column`;
                return {
                    title,
                    field,
                    width,
                    columnPoints,
                    slot,
                    notSortable,
                    formatter,
                    actualWidth: 0,
                    visible: true,
                    minWidthVisible: minWidthVisible || 0,
                    notVisible,
                    isTreeColumn,
                    headerSlot,
                    notFilterable,
                    type,
                    filterHandler,
                    filterSlot
                }
            }
        }
    },
    colors: {
        methods: {
            generateSelectBaseColors() {
                var result = [];


                result.push('#c1e9fe');
                result.push('#f9edac');
                result.push('#fdd4b3');
                result.push('#f7c6e1');
                result.push('#e1c9b7');
                result.push('#dfdfdf');
                result.push('#f9b9c3');
                result.push('#c3e9c7');
                result.push('#b5efe9');
                result.push('#c1e9fe');


                return result;
            }
        }
    }
}