
    /*!
    * tiptap-commands v1.6.0
    * (c) 2019 Scrumpy UG (limited liability)
    * @license MIT
    */
  
'use strict';

Object.defineProperty(exports, '__esModule', { value: true });

var prosemirrorCommands = require('./libs/tipTap/prosemirror-commands.js');
var prosemirrorSchemaList = require('./libs/tipTap/prosemirror-schema-list.js');
var prosemirrorInputrules = require('./libs/tipTap/prosemirror-inputrules.js');
var prosemirrorState = require('./libs/tipTap/prosemirror-state.js');
var prosemirrorModel = require('./libs/tipTap/prosemirror-model.js');
var tiptapUtils = require('./libs/tipTap/tiptap-utils.js');

function insertText () {
  var text = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : '';
  return function (state, dispatch) {
    var $from = state.selection.$from;
    var pos = $from.pos.pos;
    dispatch(state.tr.insertText(text, pos));
    return true;
  };
}

function markInputRule (regexp, markType, getAttrs) {
  return new prosemirrorInputrules.InputRule(regexp, function (state, match, start, end) {
    var attrs = getAttrs instanceof Function ? getAttrs(match) : getAttrs;
    var tr = state.tr;
    var markEnd = end;

    if (match[1]) {
      var startSpaces = match[0].search(/\S/);
      var textStart = start + match[0].indexOf(match[1]);
      var textEnd = textStart + match[1].length;

      if (textEnd < end) {
        tr.delete(textEnd, end);
      }

      if (textStart > start) {
        tr.delete(start + startSpaces, textStart);
      }

      markEnd = start + startSpaces + match[1].length;
    }

    tr.addMark(start, markEnd, markType.create(attrs));
    tr.removeStoredMark(markType); // Do not continue with mark.

    return tr;
  });
}

function nodeInputRule (regexp, type, getAttrs) {
  return new prosemirrorInputrules.InputRule(regexp, function (state, match, start, end) {
    var attrs = getAttrs instanceof Function ? getAttrs(match) : getAttrs;
    var tr = state.tr;

    if (match[0]) {
      tr.replaceWith(start - 1, end, type.create(attrs));
    }

    return tr;
  });
}

function pasteRule (regexp, type, getAttrs) {
  var handler = function handler(fragment) {
    var nodes = [];
    fragment.forEach(function (child) {
      if (child.isText) {
        var text = child.text;
        var pos = 0;
        var match;

        do {
          match = regexp.exec(text);

          if (match) {
            var start = match.index;
            var end = start + match[0].length;
            var attrs = getAttrs instanceof Function ? getAttrs(match[0]) : getAttrs;

            if (start > 0) {
              nodes.push(child.cut(pos, start));
            }

            nodes.push(child.cut(start, end).mark(type.create(attrs).addToSet(child.marks)));
            pos = end;
          }
        } while (match);

        if (pos < text.length) {
          nodes.push(child.cut(pos));
        }
      } else {
        nodes.push(child.copy(handler(child.content)));
      }
    });
    return prosemirrorModel.Fragment.fromArray(nodes);
  };

  return new prosemirrorState.Plugin({
    props: {
      transformPasted: function transformPasted(slice) {
        return new prosemirrorModel.Slice(handler(slice.content), slice.openStart, slice.openEnd);
      }
    }
  });
}

function markPasteRule (regexp, type, getAttrs) {
  var handler = function handler(fragment) {
    var nodes = [];
    fragment.forEach(function (child) {
      if (child.isText) {
        var text = child.text;
        var pos = 0;
        var match; // eslint-disable-next-line

        while ((match = regexp.exec(text)) !== null) {
          if (match[1]) {
            var start = match.index;
            var end = start + match[0].length;
            var textStart = start + match[0].indexOf(match[1]);
            var textEnd = textStart + match[1].length;
            var attrs = getAttrs instanceof Function ? getAttrs(match) : getAttrs; // adding text before markdown to nodes

            if (start > 0) {
              nodes.push(child.cut(pos, start));
            } // adding the markdown part to nodes


            nodes.push(child.cut(textStart, textEnd).mark(type.create(attrs).addToSet(child.marks)));
            pos = end;
          }
        } // adding rest of text to nodes


        if (pos < text.length) {
          nodes.push(child.cut(pos));
        }
      } else {
        nodes.push(child.copy(handler(child.content)));
      }
    });
    return prosemirrorModel.Fragment.fromArray(nodes);
  };

  return new prosemirrorState.Plugin({
    props: {
      transformPasted: function transformPasted(slice) {
        return new prosemirrorModel.Slice(handler(slice.content), slice.openStart, slice.openEnd);
      }
    }
  });
}

function removeMark (type) {
  return function (state, dispatch) {
    var _state$selection = state.selection,
        from = _state$selection.from,
        to = _state$selection.to;
    return dispatch(state.tr.removeMark(from, to, type));
  };
}

function replaceText () {
  var range = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : null;
  var type = arguments.length > 1 ? arguments[1] : undefined;
  var attrs = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : {};
  return function (state, dispatch) {
    var _state$selection = state.selection,
        $from = _state$selection.$from,
        $to = _state$selection.$to;
    var index = $from.index();
    var from = range ? range.from : $from.pos;
    var to = range ? range.to : $to.pos;

    if (!$from.parent.canReplaceWith(index, index, type)) {
      return false;
    }

    if (dispatch) {
      dispatch(state.tr.replaceWith(from, to, type.create(attrs)));
    }

    return true;
  };
}

function setInlineBlockType (type) {
  var attrs = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : {};
  return function (state, dispatch) {
    var $from = state.selection.$from;
    var index = $from.index();

    if (!$from.parent.canReplaceWith(index, index, type)) {
      return false;
    }

    if (dispatch) {
      dispatch(state.tr.replaceSelectionWith(type.create(attrs)));
    }

    return true;
  };
}

// this is a copy of canSplit
// see https://github.com/ProseMirror/prosemirror-transform/blob/master/src/structure.js
// Since this piece of code was "borrowed" from prosemirror, ESLint rules are ignored.

/* eslint-disable max-len, no-plusplus, no-undef, eqeqeq */
function canSplit(doc, pos) {
  var depth = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : 1;
  var typesAfter = arguments.length > 3 ? arguments[3] : undefined;
  var $pos = doc.resolve(pos);
  var base = $pos.depth - depth;
  var innerType = typesAfter && typesAfter[typesAfter.length - 1] || $pos.parent;
  if (base < 0 || $pos.parent.type.spec.isolating || !$pos.parent.canReplace($pos.index(), $pos.parent.childCount) || !innerType.type.validContent($pos.parent.content.cutByIndex($pos.index(), $pos.parent.childCount))) return false;

  for (var d = $pos.depth - 1, i = depth - 2; d > base; d--, i--) {
    var node = $pos.node(d);

    var _index = $pos.index(d);

    if (node.type.spec.isolating) return false;
    var rest = node.content.cutByIndex(_index, node.childCount);
    var after = typesAfter && typesAfter[i] || node;
    if (after != node) rest = rest.replaceChild(0, after.type.create(after.attrs));
    /* Change starts from here */
    // if (!node.canReplace(index + 1, node.childCount) || !after.type.validContent(rest))
    //   return false

    if (!node.canReplace(_index + 1, node.childCount)) return false;
    /* Change ends here */
  }

  var index = $pos.indexAfter(base);
  var baseType = typesAfter && typesAfter[0];
  return $pos.node(base).canReplaceWith(index, index, baseType ? baseType.type : $pos.node(base + 1).type);
} // this is a copy of splitListItem
// see https://github.com/ProseMirror/prosemirror-schema-list/blob/master/src/schema-list.js


function splitListItem(itemType) {
  return function _splitListItem(state, dispatch) {
    var _state$selection = state.selection,
        $from = _state$selection.$from,
        $to = _state$selection.$to,
        node = _state$selection.node;
    if (node && node.isBlock || $from.depth < 2 || !$from.sameParent($to)) return false;
    var grandParent = $from.node(-1);
    if (grandParent.type != itemType) return false;

    if ($from.parent.content.size == 0) {
      // In an empty block. If this is a nested list, the wrapping
      // list item should be split. Otherwise, bail out and let next
      // command handle lifting.
      if ($from.depth == 2 || $from.node(-3).type != itemType || $from.index(-2) != $from.node(-2).childCount - 1) return false;

      if (dispatch) {
        var wrap = Fragment.empty;
        var keepItem = $from.index(-1) > 0; // Build a fragment containing empty versions of the structure
        // from the outer list item to the parent node of the cursor

        for (var d = $from.depth - (keepItem ? 1 : 2); d >= $from.depth - 3; d--) {
          wrap = Fragment.from($from.node(d).copy(wrap));
        } // Add a second list item with an empty default start node


        wrap = wrap.append(Fragment.from(itemType.createAndFill()));

        var _tr = state.tr.replace($from.before(keepItem ? null : -1), $from.after(-3), new Slice(wrap, keepItem ? 3 : 2, 2));

        _tr.setSelection(state.selection.constructor.near(_tr.doc.resolve($from.pos + (keepItem ? 3 : 2))));

        dispatch(_tr.scrollIntoView());
      }

      return true;
    }

    var nextType = $to.pos == $from.end() ? grandParent.contentMatchAt($from.indexAfter(-1)).defaultType : null;
    var tr = state.tr.delete($from.pos, $to.pos);
    /* Change starts from here */
    // let types = nextType && [null, {type: nextType}]

    var types = nextType && [{
      type: itemType
    }, {
      type: nextType
    }];
    if (!types) types = [{
      type: itemType
    }, null];
    /* Change ends here */

    if (!canSplit(tr.doc, $from.pos, 2, types)) return false;
    if (dispatch) dispatch(tr.split($from.pos, 2, [{
      type: state.schema.nodes.todo_item,
      attrs: {
        done: false
      }
    }]).scrollIntoView());
    return true;
  };
}
/* eslint-enable max-len, no-plusplus, no-undef, eqeqeq */

function toggleBlockType (type, toggletype) {
  var attrs = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : {};
  return function (state, dispatch, view) {
    var isActive = tiptapUtils.nodeIsActive(state, type, attrs);

    if (isActive) {
      return prosemirrorCommands.setBlockType(toggletype)(state, dispatch, view);
    }

    return prosemirrorCommands.setBlockType(type, attrs)(state, dispatch, view);
  };
}

function toggleList(type, itemType) {
  return function (state, dispatch, view) {
    var isActive = tiptapUtils.nodeIsActive(state, type);

    if (isActive) {
      return prosemirrorSchemaList.liftListItem(itemType)(state, dispatch, view);
    }

    return prosemirrorSchemaList.wrapInList(type)(state, dispatch, view);
  };
} // https://discuss.prosemirror.net/t/list-type-toggle/948
// import { wrapInList, liftListItem } from 'prosemirror-schema-list'
// function isList(node, schema) {
//   return (node.type === schema.nodes.bullet_list || node.type === schema.nodes.ordered_list)
// }
// export default function toggleList(listType, schema) {
//   const lift = liftListItem(schema.nodes.list_item)
// 	const wrap = wrapInList(listType)
//   return (state, dispatch) => {
//     const { $from, $to } = state.selection
//     const range = $from.blockRange($to)
//     if (!range) {
//       return false
// 		}
//     if (range.depth >= 2 && $from.node(range.depth - 1).type === listType) {
//       return lift(state, dispatch)
//     } else if (range.depth >= 2 && isList($from.node(range.depth - 1), schema)) {
//       const tr = state.tr
// 			const node = $from.before(range.depth - 1)
// 			console.log({node})
//       // TODO: how do I pass the node above to `setNodeType`?
//       // tr.setNodeType(range.start, listType);
//       if (dispatch) dispatch(tr)
//       return false
//     } else {
//       return wrap(state, dispatch)
//     }
//   }
// }

function toggleWrap (type) {
  return function (state, dispatch, view) {
    var isActive = tiptapUtils.nodeIsActive(state, type);

    if (isActive) {
      return prosemirrorCommands.lift(state, dispatch);
    }

    return prosemirrorCommands.wrapIn(type)(state, dispatch, view);
  };
}

function updateMark (type, attrs) {
  return function (state, dispatch) {
    var _state$selection = state.selection,
        from = _state$selection.from,
        to = _state$selection.to;
    return dispatch(state.tr.addMark(from, to, type.create(attrs)));
  };
}

exports.chainCommands = prosemirrorCommands.chainCommands;
exports.deleteSelection = prosemirrorCommands.deleteSelection;
exports.joinBackward = prosemirrorCommands.joinBackward;
exports.selectNodeBackward = prosemirrorCommands.selectNodeBackward;
exports.joinForward = prosemirrorCommands.joinForward;
exports.selectNodeForward = prosemirrorCommands.selectNodeForward;
exports.joinUp = prosemirrorCommands.joinUp;
exports.joinDown = prosemirrorCommands.joinDown;
exports.lift = prosemirrorCommands.lift;
exports.newlineInCode = prosemirrorCommands.newlineInCode;
exports.exitCode = prosemirrorCommands.exitCode;
exports.createParagraphNear = prosemirrorCommands.createParagraphNear;
exports.liftEmptyBlock = prosemirrorCommands.liftEmptyBlock;
exports.splitBlock = prosemirrorCommands.splitBlock;
exports.splitBlockKeepMarks = prosemirrorCommands.splitBlockKeepMarks;
exports.selectParentNode = prosemirrorCommands.selectParentNode;
exports.selectAll = prosemirrorCommands.selectAll;
exports.wrapIn = prosemirrorCommands.wrapIn;
exports.setBlockType = prosemirrorCommands.setBlockType;
exports.toggleMark = prosemirrorCommands.toggleMark;
exports.autoJoin = prosemirrorCommands.autoJoin;
exports.baseKeymap = prosemirrorCommands.baseKeymap;
exports.pcBaseKeymap = prosemirrorCommands.pcBaseKeymap;
exports.macBaseKeymap = prosemirrorCommands.macBaseKeymap;
exports.addListNodes = prosemirrorSchemaList.addListNodes;
exports.wrapInList = prosemirrorSchemaList.wrapInList;
exports.splitListItem = prosemirrorSchemaList.splitListItem;
exports.liftListItem = prosemirrorSchemaList.liftListItem;
exports.sinkListItem = prosemirrorSchemaList.sinkListItem;
exports.wrappingInputRule = prosemirrorInputrules.wrappingInputRule;
exports.textblockTypeInputRule = prosemirrorInputrules.textblockTypeInputRule;
exports.insertText = insertText;
exports.markInputRule = markInputRule;
exports.markPasteRule = markPasteRule;
exports.nodeInputRule = nodeInputRule;
exports.pasteRule = pasteRule;
exports.removeMark = removeMark;
exports.replaceText = replaceText;
exports.setInlineBlockType = setInlineBlockType;
exports.splitToDefaultListItem = splitListItem;
exports.toggleBlockType = toggleBlockType;
exports.toggleList = toggleList;
exports.toggleWrap = toggleWrap;
exports.updateMark = updateMark;
