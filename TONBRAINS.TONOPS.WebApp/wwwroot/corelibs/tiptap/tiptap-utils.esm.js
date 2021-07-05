
    /*!
    * tiptap-utils v1.2.0
    * (c) 2019 Scrumpy UG (limited liability)
    * @license MIT
    */
  
import { findParentNode } from './libs/tipTap/prosemirror-utils.js';

function _toConsumableArray(arr) {
  return _arrayWithoutHoles(arr) || _iterableToArray(arr) || _nonIterableSpread();
}

function _arrayWithoutHoles(arr) {
  if (Array.isArray(arr)) {
    for (var i = 0, arr2 = new Array(arr.length); i < arr.length; i++) arr2[i] = arr[i];

    return arr2;
  }
}

function _iterableToArray(iter) {
  if (Symbol.iterator in Object(iter) || Object.prototype.toString.call(iter) === "[object Arguments]") return Array.from(iter);
}

function _nonIterableSpread() {
  throw new TypeError("Invalid attempt to spread non-iterable instance");
}

function getMarkAttrs (state, type) {
  var _state$selection = state.selection,
      from = _state$selection.from,
      to = _state$selection.to;
  var marks = [];
  state.doc.nodesBetween(from, to, function (node) {
    marks = _toConsumableArray(marks).concat(_toConsumableArray(node.marks));
  });
  var mark = marks.find(function (markItem) {
    return markItem.type.name === type.name;
  });

  if (mark) {
    return mark.attrs;
  }

  return {};
}

function getMarkRange () {
  var $pos = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : null;
  var type = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : null;

  if (!$pos || !type) {
    return false;
  }

  var start = $pos.parent.childAfter($pos.parentOffset);

  if (!start.node) {
    return false;
  }

  var link = start.node.marks.find(function (mark) {
    return mark.type === type;
  });

  if (!link) {
    return false;
  }

  var startIndex = $pos.index();
  var startPos = $pos.start() + start.offset;

  while (startIndex > 0 && link.isInSet($pos.parent.child(startIndex - 1).marks)) {
    startIndex -= 1;
    startPos -= $pos.parent.child(startIndex).nodeSize;
  } // const endIndex = $pos.indexAfter()


  var endPos = startPos + start.node.nodeSize; // disable for now. see #156
  // while (endIndex < $pos.parent.childCount && link.isInSet($pos.parent.child(endIndex).marks)) {
  //   endPos += $pos.parent.child(endIndex).nodeSize
  //   endIndex += 1
  // }

  return {
    from: startPos,
    to: endPos
  };
}

function markIsActive (state, type) {
  var _state$selection = state.selection,
      from = _state$selection.from,
      $from = _state$selection.$from,
      to = _state$selection.to,
      empty = _state$selection.empty;

  if (empty) {
    return !!type.isInSet(state.storedMarks || $from.marks());
  }

  return !!state.doc.rangeHasMark(from, to, type);
}

function nodeIsActive (state, type) {
  var attrs = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : {};

  var predicate = function predicate(node) {
    return node.type === type;
  };

  var parent = findParentNode(predicate)(state.selection);

  if (!Object.keys(attrs).length || !parent) {
    return !!parent;
  }

  return parent.node.hasMarkup(type, attrs);
}

export { getMarkAttrs, getMarkRange, markIsActive, nodeIsActive };
