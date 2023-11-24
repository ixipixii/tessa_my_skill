/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = "./src/fdProcesses/index.ts");
/******/ })
/************************************************************************/
/******/ ({

/***/ "./src/fdProcesses/fdCardUIExtension.ts":
/*!**********************************************!*\
  !*** ./src/fdProcesses/fdCardUIExtension.ts ***!
  \**********************************************/
/*! exports provided: FdCardUIExtension */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FdCardUIExtension", function() { return FdCardUIExtension; });
/* harmony import */ var tessa_ui_cards__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tessa/ui/cards */ "tessa/ui/cards");
/* harmony import */ var tessa_ui_cards__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(tessa_ui_cards__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var tessa_ui__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! tessa/ui */ "tessa/ui");
/* harmony import */ var tessa_ui__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(tessa_ui__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var mobx__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! mobx */ "mobx");
/* harmony import */ var mobx__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(mobx__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var _fdProcessTilesExtension__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./fdProcessTilesExtension */ "./src/fdProcesses/fdProcessTilesExtension.ts");
var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }





var FdCardUIExtension = function (_CardUIExtension) {
    _inherits(FdCardUIExtension, _CardUIExtension);

    function FdCardUIExtension() {
        _classCallCheck(this, FdCardUIExtension);

        var _this = _possibleConstructorReturn(this, (FdCardUIExtension.__proto__ || Object.getPrototypeOf(FdCardUIExtension)).apply(this, arguments));

        _this._disposer = null;
        return _this;
    }

    _createClass(FdCardUIExtension, [{
        key: 'initialized',
        value: function initialized(context) {
            if (!context.model || !context.model.card) {
                return;
            }
            var card = context.model.card;
            // скроем кнопки заданий
            this.HideTaskOptions(context, card);
            // Добавим кнопку визуализации процесса
            this.AddVisualizeButton(context, card);
        }
    }, {
        key: 'finalized',
        value: function finalized() {
            // подчищаем за собой
            if (this._disposer) {
                this._disposer();
                this._disposer = null;
            }
        }
    }, {
        key: 'AddVisualizeButton',
        value: function AddVisualizeButton(context, card) {
            // пытаемся найти грид по алиасу
            var table = context.model.controls.get('FdProcessesTable');
            if (!table) {
                return;
            }
            // создаем кнопку визуализации процесса
            var copyButton = new tessa_ui__WEBPACK_IMPORTED_MODULE_1__["UIButton"]('Визуализировать', function () {
                // получаем текущую выбранную строку
                var selectedRow = table.selectedRow;
                if (!selectedRow) {
                    return;
                }
                // визуализируем экземпляр процесса
                _fdProcessTilesExtension__WEBPACK_IMPORTED_MODULE_3__["FdProcessTilesExtension"].VisualizeProcess(card.id, null, selectedRow.rowId);
            });
            table.leftButtons.push(copyButton);
            // следим за selectedRow; когда значение меняется, то мы устанавливаем доступность кнопки
            this._disposer = Object(mobx__WEBPACK_IMPORTED_MODULE_2__["autorun"])(function () {
                return copyButton.setIsEnabled(!!table.selectedRow);
            });
        }
    }, {
        key: 'HideTaskOptions',
        value: function HideTaskOptions(context, card) {
            var _this2 = this;

            var hiddenOptionsList = Object(tessa_ui__WEBPACK_IMPORTED_MODULE_1__["tryGetFromInfo"])(card.info, 'FdHiddenTaskOptionsKey', []);
            if (!hiddenOptionsList || hiddenOptionsList.length === 0) {
                return;
            }
            // пытаемся получить все задания
            var tasks = context.model.tryGetTasks();
            if (!tasks) {
                return;
            }
            var _iteratorNormalCompletion = true;
            var _didIteratorError = false;
            var _iteratorError = undefined;

            try {
                for (var _iterator = tasks[Symbol.iterator](), _step; !(_iteratorNormalCompletion = (_step = _iterator.next()).done); _iteratorNormalCompletion = true) {
                    var task = _step.value;

                    // при инциализации формы
                    this.removeHiddenActions(hiddenOptionsList, task);
                    // подписываемся на изменения формы в задании
                    task.workspaceChanged.add(function (e) {
                        return _this2.removeHiddenActions(hiddenOptionsList, e.task);
                    });
                }
            } catch (err) {
                _didIteratorError = true;
                _iteratorError = err;
            } finally {
                try {
                    if (!_iteratorNormalCompletion && _iterator.return) {
                        _iterator.return();
                    }
                } finally {
                    if (_didIteratorError) {
                        throw _iteratorError;
                    }
                }
            }
        }
    }, {
        key: 'removeHiddenActions',
        value: function removeHiddenActions(hiddenOptionsList, task) {
            var hiddenOptionIDs = hiddenOptionsList.filter(function (x) {
                return x['TaskRowID'].$value === task.taskModel.card.id;
            }).map(function (x) {
                return x['OptionID'].$value;
            });
            hiddenOptionIDs.forEach(function (optionIdToHide) {
                var actionIndexToRemove = task.taskWorkspace.actions.findIndex(function (x) {
                    return x.completionOption !== null && x.completionOption.id === optionIdToHide;
                });
                if (actionIndexToRemove >= 0) {
                    task.taskWorkspace.actions.splice(actionIndexToRemove, 1);
                } else {
                    var additionalActionIndexToRemove = task.taskWorkspace.additionalActions.findIndex(function (x) {
                        return x.completionOption !== null && x.completionOption.id === optionIdToHide;
                    });
                    if (additionalActionIndexToRemove >= 0) {
                        task.taskWorkspace.additionalActions.splice(additionalActionIndexToRemove, 1);
                    }
                }
            });
        }
    }]);

    return FdCardUIExtension;
}(tessa_ui_cards__WEBPACK_IMPORTED_MODULE_0__["CardUIExtension"]);

/***/ }),

/***/ "./src/fdProcesses/fdCommentRequestUIExtension.ts":
/*!********************************************************!*\
  !*** ./src/fdProcesses/fdCommentRequestUIExtension.ts ***!
  \********************************************************/
/*! exports provided: FdCommentRequestUIExtension */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FdCommentRequestUIExtension", function() { return FdCommentRequestUIExtension; });
/* harmony import */ var tessa_ui_cards__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tessa/ui/cards */ "tessa/ui/cards");
/* harmony import */ var tessa_ui_cards__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(tessa_ui_cards__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var tessa_platform__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! tessa/platform */ "tessa/platform");
/* harmony import */ var tessa_platform__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(tessa_platform__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var tessa_cards_types__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! tessa/cards/types */ "tessa/cards/types");
/* harmony import */ var tessa_cards_types__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(tessa_cards_types__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var tessa_ui_cards_forms__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! tessa/ui/cards/forms */ "tessa/ui/cards/forms");
/* harmony import */ var tessa_ui_cards_forms__WEBPACK_IMPORTED_MODULE_3___default = /*#__PURE__*/__webpack_require__.n(tessa_ui_cards_forms__WEBPACK_IMPORTED_MODULE_3__);
/* harmony import */ var tessa_cards__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! tessa/cards */ "tessa/cards");
/* harmony import */ var tessa_cards__WEBPACK_IMPORTED_MODULE_4___default = /*#__PURE__*/__webpack_require__.n(tessa_cards__WEBPACK_IMPORTED_MODULE_4__);
var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }






var FdCommentRequestUIExtension = function (_CardUIExtension) {
    _inherits(FdCommentRequestUIExtension, _CardUIExtension);

    function FdCommentRequestUIExtension() {
        _classCallCheck(this, FdCommentRequestUIExtension);

        return _possibleConstructorReturn(this, (FdCommentRequestUIExtension.__proto__ || Object.getPrototypeOf(FdCommentRequestUIExtension)).apply(this, arguments));
    }

    _createClass(FdCommentRequestUIExtension, [{
        key: 'initialized',
        value: function initialized(context) {
            var cardType = context.model.cardType;
            if (Object(tessa_platform__WEBPACK_IMPORTED_MODULE_1__["hasNotFlag"])(cardType.flags, tessa_cards_types__WEBPACK_IMPORTED_MODULE_2__["CardTypeFlags"].AllowTasks)) {
                return;
            }
            var model = context.model;
            if (!(model.mainForm instanceof tessa_ui_cards_forms__WEBPACK_IMPORTED_MODULE_3__["DefaultFormTabWithTasksViewModel"])) {
                return;
            }
            var _iteratorNormalCompletion = true;
            var _didIteratorError = false;
            var _iteratorError = undefined;

            try {
                for (var _iterator = model.mainForm.tasks[Symbol.iterator](), _step; !(_iteratorNormalCompletion = (_step = _iterator.next()).done); _iteratorNormalCompletion = true) {
                    var task = _step.value;

                    var taskModel = task.taskModel;
                    if (taskModel.cardType.id === '98407b52-957e-425c-8809-e7a6c358f127' // FdRequestCommentTypeID
                    && Object(tessa_platform__WEBPACK_IMPORTED_MODULE_1__["hasNotFlag"])(taskModel.cardTask.flags, tessa_cards__WEBPACK_IMPORTED_MODULE_4__["CardTaskFlags"].Performer) && !taskModel.cardTask.isLockedEffective) {
                        var commentControl = taskModel.controls.get('Comment');
                        if (commentControl) {
                            commentControl.controlVisibility = tessa_platform__WEBPACK_IMPORTED_MODULE_1__["Visibility"].Collapsed;
                        }
                    }
                }
            } catch (err) {
                _didIteratorError = true;
                _iteratorError = err;
            } finally {
                try {
                    if (!_iteratorNormalCompletion && _iterator.return) {
                        _iterator.return();
                    }
                } finally {
                    if (_didIteratorError) {
                        throw _iteratorError;
                    }
                }
            }
        }
    }]);

    return FdCommentRequestUIExtension;
}(tessa_ui_cards__WEBPACK_IMPORTED_MODULE_0__["CardUIExtension"]);

/***/ }),

/***/ "./src/fdProcesses/fdProcessTemplateTileExtension.ts":
/*!***********************************************************!*\
  !*** ./src/fdProcesses/fdProcessTemplateTileExtension.ts ***!
  \***********************************************************/
/*! exports provided: FdProcessTemplateTileExtension */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FdProcessTemplateTileExtension", function() { return FdProcessTemplateTileExtension; });
/* harmony import */ var tessa_ui_tiles__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tessa/ui/tiles */ "tessa/ui/tiles");
/* harmony import */ var tessa_ui_tiles__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(tessa_ui_tiles__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var tessa_cards__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! tessa/cards */ "tessa/cards");
/* harmony import */ var tessa_cards__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(tessa_cards__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var tessa_ui__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! tessa/ui */ "tessa/ui");
/* harmony import */ var tessa_ui__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(tessa_ui__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var _fdProcessTilesExtension__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./fdProcessTilesExtension */ "./src/fdProcesses/fdProcessTilesExtension.ts");
var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var __awaiter = undefined && undefined.__awaiter || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) {
            try {
                step(generator.next(value));
            } catch (e) {
                reject(e);
            }
        }
        function rejected(value) {
            try {
                step(generator["throw"](value));
            } catch (e) {
                reject(e);
            }
        }
        function step(result) {
            result.done ? resolve(result.value) : new P(function (resolve) {
                resolve(result.value);
            }).then(fulfilled, rejected);
        }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};




var FdProcessTemplateTileExtension = function (_TileExtension) {
    _inherits(FdProcessTemplateTileExtension, _TileExtension);

    function FdProcessTemplateTileExtension() {
        _classCallCheck(this, FdProcessTemplateTileExtension);

        return _possibleConstructorReturn(this, (FdProcessTemplateTileExtension.__proto__ || Object.getPrototypeOf(FdProcessTemplateTileExtension)).apply(this, arguments));
    }

    _createClass(FdProcessTemplateTileExtension, [{
        key: 'initializingGlobal',
        value: function initializingGlobal(context) {
            var contextSource = context.workspace.leftPanel.contextSource;
            context.workspace.leftPanel.tiles.push(new tessa_ui_tiles__WEBPACK_IMPORTED_MODULE_0__["Tile"]({
                name: 'FdVisualizeProcessTemplateTile',
                caption: 'Визуализировать',
                icon: 'ta icon-thin-355',
                contextSource: contextSource,
                group: tessa_ui_tiles__WEBPACK_IMPORTED_MODULE_0__["TileGroups"].Cards,
                command: FdProcessTemplateTileExtension.visualizeProcessHandler,
                order: 10,
                evaluating: FdProcessTemplateTileExtension.enableIfProcessTemplateSaved
            }));
        }
    }], [{
        key: 'enableIfProcessTemplateSaved',
        value: function enableIfProcessTemplateSaved(e) {
            var editor = e.currentTile.context.cardEditor;
            var model = void 0;
            e.setIsEnabledWithCollapsing(e.currentTile, !!editor && !!(model = editor.cardModel) && model.cardType.id === 'd0a1425a-f233-4d75-80af-8b6d5cc529b4' // FdProcessTemplateID
            && model.card.storeMode === tessa_cards__WEBPACK_IMPORTED_MODULE_1__["CardStoreMode"].Update);
        }
    }, {
        key: 'visualizeProcessHandler',
        value: function visualizeProcessHandler() {
            return __awaiter(this, void 0, void 0, /*#__PURE__*/regeneratorRuntime.mark(function _callee() {
                var context, editor, model, processTemplateID;
                return regeneratorRuntime.wrap(function _callee$(_context) {
                    while (1) {
                        switch (_context.prev = _context.next) {
                            case 0:
                                context = tessa_ui__WEBPACK_IMPORTED_MODULE_2__["UIContext"].current;
                                editor = context.cardEditor;

                                if (!(!editor || !editor.cardModel)) {
                                    _context.next = 4;
                                    break;
                                }

                                return _context.abrupt('return');

                            case 4:
                                model = editor.cardModel;
                                // визуализируем шаблон процесса

                                processTemplateID = model.card.id;

                                _fdProcessTilesExtension__WEBPACK_IMPORTED_MODULE_3__["FdProcessTilesExtension"].VisualizeProcess(null, processTemplateID, null);

                            case 7:
                            case 'end':
                                return _context.stop();
                        }
                    }
                }, _callee, this);
            }));
        }
    }]);

    return FdProcessTemplateTileExtension;
}(tessa_ui_tiles__WEBPACK_IMPORTED_MODULE_0__["TileExtension"]);

/***/ }),

/***/ "./src/fdProcesses/fdProcessTilesExtension.ts":
/*!****************************************************!*\
  !*** ./src/fdProcesses/fdProcessTilesExtension.ts ***!
  \****************************************************/
/*! exports provided: FdProcessTilesContainer, FdProcessTilesInitalizationExtension, FdProcessTilesExtension */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FdProcessTilesContainer", function() { return FdProcessTilesContainer; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FdProcessTilesInitalizationExtension", function() { return FdProcessTilesInitalizationExtension; });
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FdProcessTilesExtension", function() { return FdProcessTilesExtension; });
/* harmony import */ var tessa__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tessa */ "tessa");
/* harmony import */ var tessa__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(tessa__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var tessa_ui_tiles__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! tessa/ui/tiles */ "tessa/ui/tiles");
/* harmony import */ var tessa_ui_tiles__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(tessa_ui_tiles__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var tessa_ui__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! tessa/ui */ "tessa/ui");
/* harmony import */ var tessa_ui__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(tessa_ui__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var tessa_cards_service__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! tessa/cards/service */ "tessa/cards/service");
/* harmony import */ var tessa_cards_service__WEBPACK_IMPORTED_MODULE_3___default = /*#__PURE__*/__webpack_require__.n(tessa_cards_service__WEBPACK_IMPORTED_MODULE_3__);
/* harmony import */ var tessa_cards__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! tessa/cards */ "tessa/cards");
/* harmony import */ var tessa_cards__WEBPACK_IMPORTED_MODULE_4___default = /*#__PURE__*/__webpack_require__.n(tessa_cards__WEBPACK_IMPORTED_MODULE_4__);
/* harmony import */ var tessa_platform__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! tessa/platform */ "tessa/platform");
/* harmony import */ var tessa_platform__WEBPACK_IMPORTED_MODULE_5___default = /*#__PURE__*/__webpack_require__.n(tessa_platform__WEBPACK_IMPORTED_MODULE_5__);
var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

var __awaiter = undefined && undefined.__awaiter || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) {
            try {
                step(generator.next(value));
            } catch (e) {
                reject(e);
            }
        }
        function rejected(value) {
            try {
                step(generator["throw"](value));
            } catch (e) {
                reject(e);
            }
        }
        function step(result) {
            result.done ? resolve(result.value) : new P(function (resolve) {
                resolve(result.value);
            }).then(fulfilled, rejected);
        }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};






var FdProcessTilesContainer = function () {
    function FdProcessTilesContainer() {
        _classCallCheck(this, FdProcessTilesContainer);
    }

    _createClass(FdProcessTilesContainer, [{
        key: 'init',
        value: function init(tilesStorage) {
            this._tilesStorage = tilesStorage;
        }
    }, {
        key: 'tiles',
        get: function get() {
            return this._tilesStorage || [];
        }
    }], [{
        key: 'instance',
        get: function get() {
            if (!FdProcessTilesContainer._instance) {
                FdProcessTilesContainer._instance = new FdProcessTilesContainer();
            }
            return FdProcessTilesContainer._instance;
        }
    }]);

    return FdProcessTilesContainer;
}();
var FdProcessTilesInitalizationExtension = function (_ApplicationExtension) {
    _inherits(FdProcessTilesInitalizationExtension, _ApplicationExtension);

    function FdProcessTilesInitalizationExtension() {
        _classCallCheck(this, FdProcessTilesInitalizationExtension);

        return _possibleConstructorReturn(this, (FdProcessTilesInitalizationExtension.__proto__ || Object.getPrototypeOf(FdProcessTilesInitalizationExtension)).apply(this, arguments));
    }

    _createClass(FdProcessTilesInitalizationExtension, [{
        key: 'afterMetadataReceived',
        value: function afterMetadataReceived(context) {
            if (context.mainPartResponse) {
                var tiles = Object(tessa_ui__WEBPACK_IMPORTED_MODULE_2__["tryGetFromInfo"])(context.mainPartResponse.info, 'FdAllPossibleProcesses', []);
                FdProcessTilesContainer.instance.init(tiles);
            }
        }
    }]);

    return FdProcessTilesInitalizationExtension;
}(tessa__WEBPACK_IMPORTED_MODULE_0__["ApplicationExtension"]);
var FdProcessTilesExtension = function (_TileExtension) {
    _inherits(FdProcessTilesExtension, _TileExtension);

    function FdProcessTilesExtension() {
        _classCallCheck(this, FdProcessTilesExtension);

        return _possibleConstructorReturn(this, (FdProcessTilesExtension.__proto__ || Object.getPrototypeOf(FdProcessTilesExtension)).apply(this, arguments));
    }

    _createClass(FdProcessTilesExtension, [{
        key: 'initializingGlobal',
        value: function initializingGlobal(context) {
            var tiles = FdProcessTilesContainer.instance.tiles;
            FdProcessTilesExtension.createProcessActionTiles(context, tiles);
        }
    }], [{
        key: 'createProcessActionTiles',
        value: function createProcessActionTiles(context, tiles) {
            return __awaiter(this, void 0, void 0, /*#__PURE__*/regeneratorRuntime.mark(function _callee() {
                var order, _iteratorNormalCompletion, _didIteratorError, _iteratorError, _iterator, _step, tile, processID, processName, processStartTileCaption, processNameIcon, processStartTileIcon, processNameIconNumber, processStartTileIconNumber, groupTile, startProcessTile, visualizeProcessTile;

                return regeneratorRuntime.wrap(function _callee$(_context) {
                    while (1) {
                        switch (_context.prev = _context.next) {
                            case 0:
                                if (!Array.isArray(tiles)) {
                                    _context.next = 21;
                                    break;
                                }

                                order = 100;
                                _iteratorNormalCompletion = true;
                                _didIteratorError = false;
                                _iteratorError = undefined;
                                _context.prev = 5;

                                for (_iterator = tiles[Symbol.iterator](); !(_iteratorNormalCompletion = (_step = _iterator.next()).done); _iteratorNormalCompletion = true) {
                                    tile = _step.value;
                                    processID = tile.ID;
                                    processName = tile.Name;
                                    processStartTileCaption = tile.StartTileCaption;
                                    processNameIcon = tile.NameIcon;
                                    processStartTileIcon = tile.StartTileIcon;
                                    processNameIconNumber = '';
                                    processStartTileIconNumber = '';

                                    if (processNameIcon && processNameIcon.toLowerCase().indexOf('thin') > -1) {
                                        processNameIconNumber = processNameIcon.toLowerCase().split('thin')[1];
                                    }
                                    if (!processNameIconNumber) processNameIconNumber = '360';
                                    if (processStartTileIcon && processStartTileIcon.toLowerCase().indexOf('thin') > -1) {
                                        processStartTileIconNumber = processStartTileIcon.toLowerCase().split('thin')[1];
                                    }
                                    if (!processStartTileIconNumber) processStartTileIconNumber = '258';
                                    groupTile = new tessa_ui_tiles__WEBPACK_IMPORTED_MODULE_1__["Tile"]({
                                        id: processID,
                                        name: processID,
                                        caption: processName,
                                        icon: 'ta icon-thin-' + processNameIconNumber,
                                        contextSource: context.workspace.leftPanel.contextSource,
                                        group: tessa_ui_tiles__WEBPACK_IMPORTED_MODULE_1__["TileGroups"].Cards,
                                        order: order++,
                                        command: null,
                                        evaluating: FdProcessTilesExtension.enableIfProcessIsAvailable
                                    });
                                    startProcessTile = new tessa_ui_tiles__WEBPACK_IMPORTED_MODULE_1__["Tile"]({
                                        name: processID + '_StartProcess',
                                        caption: processStartTileCaption,
                                        icon: 'ta icon-thin-' + processStartTileIconNumber,
                                        contextSource: context.workspace.leftPanel.contextSource,
                                        group: tessa_ui_tiles__WEBPACK_IMPORTED_MODULE_1__["TileGroups"].Cards,
                                        order: 0,
                                        command: FdProcessTilesExtension.startProcessAction,
                                        evaluating: FdProcessTilesExtension.enableAlways
                                    });

                                    groupTile.tiles.push(startProcessTile);
                                    visualizeProcessTile = new tessa_ui_tiles__WEBPACK_IMPORTED_MODULE_1__["Tile"]({
                                        name: processID + '_VisualizeProcess',
                                        caption: 'Визуализировать',
                                        icon: 'ta icon-thin-355',
                                        contextSource: context.workspace.leftPanel.contextSource,
                                        group: tessa_ui_tiles__WEBPACK_IMPORTED_MODULE_1__["TileGroups"].Cards,
                                        order: 1,
                                        command: FdProcessTilesExtension.visualizeProcessAction,
                                        evaluating: FdProcessTilesExtension.enableAlways
                                    });

                                    groupTile.tiles.push(visualizeProcessTile);
                                    context.workspace.leftPanel.tiles.push(groupTile);
                                }
                                _context.next = 13;
                                break;

                            case 9:
                                _context.prev = 9;
                                _context.t0 = _context['catch'](5);
                                _didIteratorError = true;
                                _iteratorError = _context.t0;

                            case 13:
                                _context.prev = 13;
                                _context.prev = 14;

                                if (!_iteratorNormalCompletion && _iterator.return) {
                                    _iterator.return();
                                }

                            case 16:
                                _context.prev = 16;

                                if (!_didIteratorError) {
                                    _context.next = 19;
                                    break;
                                }

                                throw _iteratorError;

                            case 19:
                                return _context.finish(16);

                            case 20:
                                return _context.finish(13);

                            case 21:
                            case 'end':
                                return _context.stop();
                        }
                    }
                }, _callee, this, [[5, 9, 13, 21], [14,, 16, 20]]);
            }));
        }
    }, {
        key: 'enableAlways',
        value: function enableAlways(e) {
            e.setIsEnabledWithCollapsing(e.currentTile, true);
        }
    }, {
        key: 'enableIfProcessIsAvailable',
        value: function enableIfProcessIsAvailable(e) {
            var isEnabled = false;
            var editor = e.currentTile.context.cardEditor;
            if (editor && editor.cardModel && editor.cardModel.card) {
                var card = editor.cardModel.card;
                if (card.storeMode !== tessa_cards__WEBPACK_IMPORTED_MODULE_4__["CardStoreMode"].Insert) {
                    // получим доступные тайлы
                    var availableTiles = Object(tessa_ui__WEBPACK_IMPORTED_MODULE_2__["tryGetFromInfo"])(card.info, 'FdAvailableProcesses', []);
                    if (availableTiles && availableTiles.some(function (x) {
                        return x['ID'].$value === e.currentTile.id && x['CanStartProcess'].$value === true;
                    })) {
                        isEnabled = true;
                    }
                }
            }
            e.setIsEnabledWithCollapsing(e.currentTile, isEnabled);
        }
    }, {
        key: 'startProcessAction',
        value: function startProcessAction(tile) {
            return __awaiter(this, void 0, void 0, /*#__PURE__*/regeneratorRuntime.mark(function _callee2() {
                var context, editor, model, processID;
                return regeneratorRuntime.wrap(function _callee2$(_context2) {
                    while (1) {
                        switch (_context2.prev = _context2.next) {
                            case 0:
                                context = tessa_ui__WEBPACK_IMPORTED_MODULE_2__["UIContext"].current;
                                editor = context.cardEditor;
                                model = void 0;

                                if (!(!editor || !(model = editor.cardModel))) {
                                    _context2.next = 5;
                                    break;
                                }

                                return _context2.abrupt('return');

                            case 5:
                                // выделяем GUID из названия вида "GUID_StartProcess"
                                processID = tile.name.substring(0, tile.name.indexOf('_StartProcess'));

                                editor.openCard({
                                    cardId: model.card.id,
                                    cardTypeId: model.card.typeId,
                                    cardTypeName: model.card.typeName,
                                    info: { 'fd_create_start_process_task': Object(tessa_platform__WEBPACK_IMPORTED_MODULE_5__["createTypedField"])(processID, tessa_platform__WEBPACK_IMPORTED_MODULE_5__["DotNetType"].Guid) }
                                });

                            case 7:
                            case 'end':
                                return _context2.stop();
                        }
                    }
                }, _callee2, this);
            }));
        }
    }, {
        key: 'visualizeProcessAction',
        value: function visualizeProcessAction(tile) {
            // выделяем GUID из названия вида "GUID_VisualizeProcess"
            var processID = tile.name.substring(0, tile.name.indexOf('_VisualizeProcess'));
            // визуализируем шаблон процесса
            FdProcessTilesExtension.VisualizeProcess(null, processID, null);
        }
    }, {
        key: 'VisualizeProcess',
        value: function VisualizeProcess(cardID, processTemplateID, processInstanceID) {
            return __awaiter(this, void 0, void 0, /*#__PURE__*/regeneratorRuntime.mark(function _callee3() {
                var request, response, htmlData, myWindow;
                return regeneratorRuntime.wrap(function _callee3$(_context3) {
                    while (1) {
                        switch (_context3.prev = _context3.next) {
                            case 0:
                                request = new tessa_cards_service__WEBPACK_IMPORTED_MODULE_3__["CardRequest"]();

                                request.cardId = cardID;
                                request.requestType = '673F3815-43D9-4E0E-AE59-709CBEFEF972';
                                if (processTemplateID) {
                                    request.info = { 'FdProcessTemplateID': Object(tessa_platform__WEBPACK_IMPORTED_MODULE_5__["createTypedField"])(processTemplateID, tessa_platform__WEBPACK_IMPORTED_MODULE_5__["DotNetType"].Guid) };
                                }
                                if (processInstanceID) {
                                    request.info = { 'FdProcessInstanceRowID': Object(tessa_platform__WEBPACK_IMPORTED_MODULE_5__["createTypedField"])(processInstanceID, tessa_platform__WEBPACK_IMPORTED_MODULE_5__["DotNetType"].Guid) };
                                }
                                _context3.next = 7;
                                return tessa_cards_service__WEBPACK_IMPORTED_MODULE_3__["CardService"].instance.request(request);

                            case 7:
                                response = _context3.sent;

                                if (response.validationResult.isSuccessful) {
                                    _context3.next = 12;
                                    break;
                                }

                                _context3.next = 11;
                                return Object(tessa_ui__WEBPACK_IMPORTED_MODULE_2__["showNotEmpty"])(response.validationResult.build());

                            case 11:
                                return _context3.abrupt('return');

                            case 12:
                                htmlData = Object(tessa_ui__WEBPACK_IMPORTED_MODULE_2__["tryGetFromInfo"])(response.info, 'FdHtmlData', undefined);

                                if (htmlData) {
                                    myWindow = window.open('', 'processTemplate_' + processTemplateID, '');

                                    if (myWindow && myWindow.document) {
                                        myWindow.document.open();
                                        myWindow.document.write(htmlData);
                                        myWindow.document.close();
                                    }
                                }

                            case 14:
                            case 'end':
                                return _context3.stop();
                        }
                    }
                }, _callee3, this);
            }));
        }
    }]);

    return FdProcessTilesExtension;
}(tessa_ui_tiles__WEBPACK_IMPORTED_MODULE_1__["TileExtension"]);

/***/ }),

/***/ "./src/fdProcesses/fdUIExtension.ts":
/*!******************************************!*\
  !*** ./src/fdProcesses/fdUIExtension.ts ***!
  \******************************************/
/*! exports provided: FdUIExtension */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "FdUIExtension", function() { return FdUIExtension; });
/* harmony import */ var tessa_ui_cards__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tessa/ui/cards */ "tessa/ui/cards");
/* harmony import */ var tessa_ui_cards__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(tessa_ui_cards__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var tessa_ui_cards_forms__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! tessa/ui/cards/forms */ "tessa/ui/cards/forms");
/* harmony import */ var tessa_ui_cards_forms__WEBPACK_IMPORTED_MODULE_1___default = /*#__PURE__*/__webpack_require__.n(tessa_ui_cards_forms__WEBPACK_IMPORTED_MODULE_1__);
/* harmony import */ var tessa_platform__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! tessa/platform */ "tessa/platform");
/* harmony import */ var tessa_platform__WEBPACK_IMPORTED_MODULE_2___default = /*#__PURE__*/__webpack_require__.n(tessa_platform__WEBPACK_IMPORTED_MODULE_2__);
/* harmony import */ var tessa_cards_types__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! tessa/cards/types */ "tessa/cards/types");
/* harmony import */ var tessa_cards_types__WEBPACK_IMPORTED_MODULE_3___default = /*#__PURE__*/__webpack_require__.n(tessa_cards_types__WEBPACK_IMPORTED_MODULE_3__);
/* harmony import */ var tessa_cards__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! tessa/cards */ "tessa/cards");
/* harmony import */ var tessa_cards__WEBPACK_IMPORTED_MODULE_4___default = /*#__PURE__*/__webpack_require__.n(tessa_cards__WEBPACK_IMPORTED_MODULE_4__);
var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }






var FdUIExtension = function (_CardUIExtension) {
    _inherits(FdUIExtension, _CardUIExtension);

    function FdUIExtension() {
        _classCallCheck(this, FdUIExtension);

        var _this = _possibleConstructorReturn(this, (FdUIExtension.__proto__ || Object.getPrototypeOf(FdUIExtension)).apply(this, arguments));

        _this.ApproveCategory = ['e0c70afe-bcc9-47e0-9b85-95755315e933', '621bd98a-4a93-4c57-bea6-845a9e04b046'];
        _this.AdditionalApprovalCategory = ['075b7969-8c3e-4415-a5bb-76c1169c7218', '35964a26-0f74-4fad-ad21-0e978d8bb765'];
        _this.GetApproveCategoryTaskTypes = function () {
            var result = [];
            var fdSettings = tessa_cards__WEBPACK_IMPORTED_MODULE_4__["CardSingletonCache"].instance.cards.get('FdSettings');
            if (!fdSettings) {
                return result;
            }
            var approveCategoryTaskTypesRows = fdSettings.sections.get('FdSettings_ApproveCategoryTaskTypes').rows;
            if (approveCategoryTaskTypesRows && approveCategoryTaskTypesRows.length > 0) {
                result = approveCategoryTaskTypesRows.map(function (x) {
                    return x.get('TaskTypeID');
                });
            }
            return result;
        };
        return _this;
    }

    _createClass(FdUIExtension, [{
        key: 'initialized',
        value: function initialized(context) {
            var model = context.model;
            if (!(model.mainForm instanceof tessa_ui_cards_forms__WEBPACK_IMPORTED_MODULE_1__["DefaultFormTabWithTasksViewModel"]) || Object(tessa_platform__WEBPACK_IMPORTED_MODULE_2__["hasNotFlag"])(model.cardType.flags, tessa_cards_types__WEBPACK_IMPORTED_MODULE_3__["CardTypeFlags"].AllowTasks)) {
                return;
            }
            var formWithTasks = model.mainForm;
            // доп. задания, которые указаны в настройках и используются как основные задания согласования/исполнения
            var approveCategoryTaskTypes = this.GetApproveCategoryTaskTypes();
            // скроем ненужные блоки в заданиях
            var _iteratorNormalCompletion = true;
            var _didIteratorError = false;
            var _iteratorError = undefined;

            try {
                for (var _iterator = formWithTasks.tasks[Symbol.iterator](), _step; !(_iteratorNormalCompletion = (_step = _iterator.next()).done); _iteratorNormalCompletion = true) {
                    var taskViewModel = _step.value;

                    this.HideTaskBlocks(taskViewModel, approveCategoryTaskTypes);
                }
            } catch (err) {
                _didIteratorError = true;
                _iteratorError = err;
            } finally {
                try {
                    if (!_iteratorNormalCompletion && _iterator.return) {
                        _iterator.return();
                    }
                } finally {
                    if (_didIteratorError) {
                        throw _iteratorError;
                    }
                }
            }
        }
    }, {
        key: 'HideTaskBlocks',
        value: function HideTaskBlocks(taskViewModel, approveCategoryTaskTypes) {
            var taskModel = taskViewModel.taskModel;
            // если в типе задания нет таблицы с дочерними заданиями, сразу выйдем
            if (this.ApproveCategory.indexOf(taskModel.cardType.id) < 0 && approveCategoryTaskTypes.indexOf(taskModel.cardType.id) < 0 && this.AdditionalApprovalCategory.indexOf(taskModel.cardType.id) < 0) {
                return;
            }
            var fcivSection = taskModel.card.sections.tryGet('FdCommentsInfoVirtual');
            var commentBlock = void 0;
            // скрываем блок с комментариями в текущем представлении
            if (fcivSection && fcivSection.rows.length === 0 && !!(commentBlock = taskModel.blocks.get('CommentsBlockShort'))) {
                // Если секция есть, но ее поля незаполнены - значит запроса комментария не было
                commentBlock.blockVisibility = tessa_platform__WEBPACK_IMPORTED_MODULE_2__["Visibility"].Collapsed;
            }
            var additionalApprovalBlock = void 0;
            var faaivSection = taskModel.card.sections.tryGet('FdAdditionalApprovalInfoVirtual');
            // скрываем блок с заданиями доп согласования в текущем представлении
            if (faaivSection && faaivSection.rows.length === 0 && !!(additionalApprovalBlock = taskModel.blocks.get('AdditionalApprovalBlockShort'))) {
                // Если секция есть, но ее поля незаполнены - значит запроса комментария не было
                additionalApprovalBlock.blockVisibility = tessa_platform__WEBPACK_IMPORTED_MODULE_2__["Visibility"].Collapsed;
            }
            taskViewModel.workspaceChanged.add(function (e) {
                var form = e.task.taskWorkspace.form;
                if (!form) {
                    return;
                }
                var blocks = form.blocks;
                var fcivSection = taskModel.card.sections.tryGet('FdCommentsInfoVirtual');
                var innerCommentBlock = void 0;
                if (fcivSection && fcivSection.rows.length === 0 && !!(innerCommentBlock = blocks.find(function (x) {
                    return x.name === 'CommentsBlockShort';
                }))) {
                    // Если секция есть, но ее поля незаполнены - значит запроса комментария не было
                    innerCommentBlock.blockVisibility = tessa_platform__WEBPACK_IMPORTED_MODULE_2__["Visibility"].Collapsed;
                }
                var faaivSection = taskModel.card.sections.tryGet('FdAdditionalApprovalInfoVirtual');
                var innerAdditionalApprovalBlock = void 0;
                if (faaivSection && faaivSection.rows.length === 0 && !!(innerAdditionalApprovalBlock = blocks.find(function (x) {
                    return x.name === 'AdditionalApprovalBlockShort';
                }))) {
                    // Если секция есть, но ее поля незаполнены - значит запроса комментария не было
                    innerAdditionalApprovalBlock.blockVisibility = tessa_platform__WEBPACK_IMPORTED_MODULE_2__["Visibility"].Collapsed;
                }
            });
        }
    }]);

    return FdUIExtension;
}(tessa_ui_cards__WEBPACK_IMPORTED_MODULE_0__["CardUIExtension"]);

/***/ }),

/***/ "./src/fdProcesses/index.ts":
/*!**********************************!*\
  !*** ./src/fdProcesses/index.ts ***!
  \**********************************/
/*! no exports provided */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _registrator__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./registrator */ "./src/fdProcesses/registrator.ts");


/***/ }),

/***/ "./src/fdProcesses/registrator.ts":
/*!****************************************!*\
  !*** ./src/fdProcesses/registrator.ts ***!
  \****************************************/
/*! no exports provided */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var tessa_extensions__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! tessa/extensions */ "tessa/extensions");
/* harmony import */ var tessa_extensions__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(tessa_extensions__WEBPACK_IMPORTED_MODULE_0__);
/* harmony import */ var _fdProcessTilesExtension__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./fdProcessTilesExtension */ "./src/fdProcesses/fdProcessTilesExtension.ts");
/* harmony import */ var _fdCardUIExtension__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./fdCardUIExtension */ "./src/fdProcesses/fdCardUIExtension.ts");
/* harmony import */ var _fdUIExtension__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./fdUIExtension */ "./src/fdProcesses/fdUIExtension.ts");
/* harmony import */ var _fdCommentRequestUIExtension__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./fdCommentRequestUIExtension */ "./src/fdProcesses/fdCommentRequestUIExtension.ts");
/* harmony import */ var _fdProcessTemplateTileExtension__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./fdProcessTemplateTileExtension */ "./src/fdProcesses/fdProcessTemplateTileExtension.ts");






tessa_extensions__WEBPACK_IMPORTED_MODULE_0__["ExtensionContainer"].instance.registerExtension({ extension: _fdProcessTilesExtension__WEBPACK_IMPORTED_MODULE_1__["FdProcessTilesInitalizationExtension"], stage: tessa_extensions__WEBPACK_IMPORTED_MODULE_0__["ExtensionStage"].AfterPlatform });
tessa_extensions__WEBPACK_IMPORTED_MODULE_0__["ExtensionContainer"].instance.registerExtension({ extension: _fdProcessTilesExtension__WEBPACK_IMPORTED_MODULE_1__["FdProcessTilesExtension"], stage: tessa_extensions__WEBPACK_IMPORTED_MODULE_0__["ExtensionStage"].AfterPlatform });
tessa_extensions__WEBPACK_IMPORTED_MODULE_0__["ExtensionContainer"].instance.registerExtension({ extension: _fdCardUIExtension__WEBPACK_IMPORTED_MODULE_2__["FdCardUIExtension"], stage: tessa_extensions__WEBPACK_IMPORTED_MODULE_0__["ExtensionStage"].AfterPlatform });
tessa_extensions__WEBPACK_IMPORTED_MODULE_0__["ExtensionContainer"].instance.registerExtension({ extension: _fdUIExtension__WEBPACK_IMPORTED_MODULE_3__["FdUIExtension"], stage: tessa_extensions__WEBPACK_IMPORTED_MODULE_0__["ExtensionStage"].AfterPlatform });
tessa_extensions__WEBPACK_IMPORTED_MODULE_0__["ExtensionContainer"].instance.registerExtension({ extension: _fdCommentRequestUIExtension__WEBPACK_IMPORTED_MODULE_4__["FdCommentRequestUIExtension"], stage: tessa_extensions__WEBPACK_IMPORTED_MODULE_0__["ExtensionStage"].AfterPlatform });
tessa_extensions__WEBPACK_IMPORTED_MODULE_0__["ExtensionContainer"].instance.registerExtension({ extension: _fdProcessTemplateTileExtension__WEBPACK_IMPORTED_MODULE_5__["FdProcessTemplateTileExtension"], stage: tessa_extensions__WEBPACK_IMPORTED_MODULE_0__["ExtensionStage"].AfterPlatform });

/***/ }),

/***/ "mobx":
/*!***********************!*\
  !*** external "mobx" ***!
  \***********************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = mobx;

/***/ }),

/***/ "tessa":
/*!******************************!*\
  !*** external "tessa.tessa" ***!
  \******************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = tessa.tessa;

/***/ }),

/***/ "tessa/cards":
/*!************************************!*\
  !*** external "tessa.tessa.cards" ***!
  \************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = tessa.tessa.cards;

/***/ }),

/***/ "tessa/cards/service":
/*!********************************************!*\
  !*** external "tessa.tessa.cards.service" ***!
  \********************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = tessa.tessa.cards.service;

/***/ }),

/***/ "tessa/cards/types":
/*!******************************************!*\
  !*** external "tessa.tessa.cards.types" ***!
  \******************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = tessa.tessa.cards.types;

/***/ }),

/***/ "tessa/extensions":
/*!*****************************************!*\
  !*** external "tessa.tessa.extensions" ***!
  \*****************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = tessa.tessa.extensions;

/***/ }),

/***/ "tessa/platform":
/*!***************************************!*\
  !*** external "tessa.tessa.platform" ***!
  \***************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = tessa.tessa.platform;

/***/ }),

/***/ "tessa/ui":
/*!*********************************!*\
  !*** external "tessa.tessa.ui" ***!
  \*********************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = tessa.tessa.ui;

/***/ }),

/***/ "tessa/ui/cards":
/*!***************************************!*\
  !*** external "tessa.tessa.ui.cards" ***!
  \***************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = tessa.tessa.ui.cards;

/***/ }),

/***/ "tessa/ui/cards/forms":
/*!*********************************************!*\
  !*** external "tessa.tessa.ui.cards.forms" ***!
  \*********************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = tessa.tessa.ui.cards.forms;

/***/ }),

/***/ "tessa/ui/tiles":
/*!***************************************!*\
  !*** external "tessa.tessa.ui.tiles" ***!
  \***************************************/
/*! no static exports found */
/***/ (function(module, exports) {

module.exports = tessa.tessa.ui.tiles;

/***/ })

/******/ });
//# sourceMappingURL=fdProcessesWebExtensions.9d6d1ea9fe0bd1ef7393.js.map