(function (factory) {
    if (typeof define === "function" && define.amd) {

        // AMD. Request jQuery and then run the plugin
        define(["jquery"], factory);

    } else {

        // Global jQuery
        factory(jQuery);

    }
}(function ($) {

    var arcKey = '';
    var updateTodoItemAction = "/Campaign/UpdateTodoItem";
    var deleteTodoItemAction = "/Campaign/DeleteTodoItem";

    var setup = {
        isInitDone: false
    };

    setup.listTemplate = 
        `<div class="jquery-todolist" style="display:none;">
            <div class="flexbox-container justify-between items-center">
                <label class="text-large">ToDo List</label>
                <div class="btn btn-sm btn-default todo-add-item"><i class="fa fa-plus"></i></div>
            </div>

            <div class="jquery-todolist-footer">
                <div class="jquery-todolist-add">
                    <span class="jquery-todolist-add-input">
                        <div class='fancy-textarea sm'>
                            <textarea class='form-control' name="j" placeholder="Add new item..." maxlength="50" style="display:none"></textarea>
                        </div>
                    </span>
                </div>

                <div class="jquery-todolist-items"></div>
            </div>
        </div>`;

    setup.itemTemplate = 
        `<div class="jquery-todolist-item">
            <div class="ui-state-default">
                <div class="jquery-todolist-item-title ui-widget-content jquery-todolist-action-edit jquery-todolist-action">
                    <span class="jquery-todolist-item-title-text"></span>
                </div>
            </div>
            <span class="jquery-todolist-item-actions-left">
                <span class="jquery-todolist-action jquery-todolist-item-checkbox">&#9744;</span>
            </span>
        </div>`;

    setup.itemEditTemplate = 
        `<div class="jquery-todolist-edit">
            <div class="jquery-todolist-edit-input">
                <input type="text" name="e" maxlength="50" value="" />
            </div>
        </div>`;

    setup.confirmButtonTemplate = 
        `<span class="jquery-todolist-button ui-state-default">
            <span class="jquery-todolist-action jquery-todolist-button-confirm">continue?</span>
            <span class="jquery-todolist-action jquery-todolist-button-cancel">&times;</span>
        </span>`;

    setup.menuTemplate = 
        `<div class="jquery-todolist-menu ui-widget-content">
            <div class="jquery-todolist-menu-actions">
                <a href="javascript:" class="jquery-todolist-action" data-todolistaction="close menu">&times</a>
            </div>
            <div class="jquery-todolist-menu-items">
                <a href="javascript:" class="ui-state-default jquery-todolist-action jquery-todolist-menu-item" data-todolistaction="toggle done">Show/Hide Done</a>
            </div>
        </div>`;

    /**
     * Creates a todoList within the given selection of elements.
     *
     * @param {Object} [options]
     * @param {String} [options.title="Todo List"]
     * @param {Array<Object>|Array<String>} [options.items]
     * @param {Array<Object>} [options.customActions=null]
     *
     * @return {jQuery}
     */
    $.fn.initTodoList = function (options, arc, items) {
        arcKey = arc;

        if (!setup.isInitDone) {

            setup.isInitDone = true;
            //$('<style>' + setup.styles + '</style>').appendTo('head');

            // Setup global listener: If a click is done outside an edit item, close it.
            $("body").on("click", function (ev) {

                var $elements = $("div.jquery-todolist-edit"),
                    $cntr;

                // close edit Forms
                if ($elements.length) {

                    $cntr = $(ev.target).closest(".jquery-todolist-action-edit");

                    if (!$cntr.length) {

                        $elements.find(".jquery-todolist-edit-save").click();

                    } else {

                        // Close any edit item that does not have the current
                        // target in inside of it.
                        $elements.each(function () {

                            if (!$.contains($cntr[0], this)) {

                                $(this).find(".jquery-todolist-edit-save").click();

                            }

                        });

                    }

                } //end: if: edit containers

                // Hide todo list menu
                $elements = $("div.jquery-todolist-menu:visible");

                if ($elements.length) {

                    // FIXME: need to hide menu if clicked outside of it.

                }


            });

        }

        var returnValue = this;

        this.each(function () {

            if (this.jqueryTodoList && typeof options === "string") {

                switch (String(options).toLowerCase()) {

                    // options = returns a copy of the internal options object.
                    case "getsetup":

                        returnValue = this.jqueryTodoList.getSetup();

                        break;

                }

                return this;

            } //end: options === string

            // Exit if this element already has a todo list
            if ($(this).hasClass("has-jquery-todolist")) {

                return this;

            }

            var
                todo = {

                    $ele: $(this),

                    opt: $.extend(
                        {},
                        {
                            title: "Todo List",
                            items: [],
                            /*removeLabel: "delete?",*/
                            newItemPlaceholder: "New Item",
                            editItemTooltip: "Click to Edit",
                            focusOnTitle: false,
                            customActions: null
                        },
                        options
                    ),

                    /**
                     * Returns the setup for this todo instance.
                     *
                     * @return {Object}
                     */
                    getSetup: function () {

                        var instSetup = {
                            title: todo.$ui.find(".jquery-todolist-title-text").text(),
                            items: []
                        };

                        todo.getItems().each(function () {

                            var $thisItem = $(this);

                            instSetup.items.push({
                                done: $thisItem.is(".jquery-todolist-item-done"),
                                title: $thisItem
                                    .find(".jquery-todolist-item-title-text")
                                    .text()
                            });

                        });

                        return instSetup;

                    }, //end: getSetup()

                    /**
                     * Adds an item to the todo list
                     *
                     * @param {Object} itemContent
                     *      An object that represents a todo list item. Object
                     *      must have a 'title' and 'done' property
                     *
                     * @return {jQuery}
                     *      the item created.
                     */
                    addItem: function (itemContent, suppressEvents, itemKey) {

                        var $newItem = $(setup.itemTemplate)
                            .find(".jquery-todolist-item-title-text")
                            .append(itemContent.title)
                            .end()
                            .addClass(
                                (itemContent.done ? "jquery-todolist-item-done" : "")
                            )
                            .find(".jquery-todolist-action-edit")
                            .attr("title", todo.opt.editItemTooltip)
                            .end()
                            .appendTo(todo.$ui.$items);
                        $($newItem).data('key', itemKey);

                        if (!suppressEvents) {

                            todo.$ele.trigger("todolist-add", options, todo.$ele);
                            todo.$ele.trigger("todolist-change", options, todo.$ele);

                        }

                        var postData = {
                            ItemKey: itemKey,
                            ArcKey: arcKey,
                            Text: itemContent.title
                        };

                        ajaxPost(postData, updateTodoItemAction, function () { });

                        return $newItem;

                    }, //end: addItem()

                    /**
                     * Gets the list of items from the UI
                     *
                     * @return {jQuery}
                     */
                    getItems: function () {

                        return todo.$ui.$items.find("div.jquery-todolist-item");

                    }, //end: getItems()

                    /**
                     * Shows the edit form for the item clicked on
                     *
                     * @param {jQuery} $ele
                     *
                     */
                    showEdit: function ($ele) {

                        var
                            $titleText = $ele.find(
                                ".jquery-todolist-item-title-text,.jquery-todolist-title-text"
                            ).eq(0),
                            $titleEdit = $(setup.itemEditTemplate)
                                .css("display", "none")
                                .find("input")
                                .val($titleText.text())
                                .end()
                                .appendTo($ele),
                            saveValue = function () {

                                $titleText.html($titleEdit.find("input").val());
                                $titleEdit.hide().delay("500").remove();
                                $titleText.show();
                                $ele.addClass("jquery-todolist-action");

                                todo.$ele.trigger("todolist-edit", options, todo.$ele);
                                todo.$ele.trigger("todolist-change", options, todo.$ele);

                                var postData = {
                                    ItemKey: $($ele).closest('.jquery-todolist-item').data('key'),
                                    ArcKey: arcKey,
                                    Text: $titleText.text()
                                };

                                ajaxPost(postData, updateTodoItemAction, function () { });

                            };

                        $ele.removeClass("jquery-todolist-action");
                        $titleText.hide();
                        $titleEdit.show().promise().then(function () {
                            $titleEdit.find("input").focus();
                        });

                        // Setup events
                        $titleEdit
                            .find(".jquery-todolist-edit-save")
                            .on("click", saveValue)
                            .end()
                            .find("input")
                            .on("keyup", function (ev) {

                                if (ev.which === 13) {
                                    saveValue();
                                }

                            });


                    }, //end: showEdit()

                    /**
                     * Sets up the list menu and stores it in todo.$ui.$menu
                     */
                    setupListMenu: function () {

                        todo.$ui.$menu = $(setup.menuTemplate)
                            .css("display", "none")
                            .appendTo(todo.$ui);

                        // Add custom actions
                        if ($.isArray(todo.opt.customActions)) {

                            var itemTemplate = todo.$ui.$menu
                                .find(".jquery-todolist-menu-items > a:first")
                                .clone(),
                                additionalItems = $();

                            $.each(todo.opt.customActions, function () {

                                if ($.isPlainObject(this)) {

                                    var thisCustomAction = this;

                                    additionalItems = additionalItems.add(
                                        itemTemplate.clone()
                                            .text(this.title || "Action?")
                                            .attr("data-todolistaction", "custom action")
                                            .click(function () {
                                                if ($.isFunction(thisCustomAction.action)) {

                                                    return thisCustomAction.action.call(
                                                        todo.$ele, options, thisCustomAction
                                                    );

                                                }
                                            })
                                    );

                                }

                            });

                            todo.$ui.$menu.find(".jquery-todolist-menu-items")
                                .append(additionalItems);

                        }


                    } //end: setupListMenu()

                }; //end: todo instance

            todo.$ui = $(setup.listTemplate)
                .appendTo(
                    todo.$ele.empty().addClass("has-jquery-todolist")
                )
                .find(".jquery-todolist-title-text")
                .html(todo.opt.title)
                .end()
                .find(".jquery-todolist-add-input-text")
                .attr("placeholder", todo.opt.newItemPlaceholder)
                .end()
                .find(".jquery-todolist-action-edit")
                .attr("title", todo.opt.editItemTooltip)
                .end();

            // Add references to the UI elements
            todo.$ui.$menu = null;
            todo.$ui.$items = todo.$ui.find("div.jquery-todolist-items");
            todo.$ui.$newItemInput = todo.$ui.find(".jquery-todolist-add-input textarea");
            /*todo.$ui.$addButton = todo.$ui.find(".jquery-todolist-add-action");*/

            // If there are items defined on input, create them
            if ($.isArray(todo.opt.items)) {

                $.each(todo.opt.items, function (i, item) {

                    var thisItem = {
                        title: item.Text,
                        done: false
                    };

                    //if ($.isPlainObject(item)) {

                    //    $.extend(thisItem, item);

                    //} else {

                    //    thisItem.title = String(item) || "Item " + (i + 1);
                    //}

                    todo.addItem(thisItem, true, item.ItemKey);

                });

            }

            // Bind CLICK event handler
            todo.$ui.on("click.jqueryTodoList", ".todo-add-item", function () {
                todo.$ui.$newItemInput.show('fast');
                todo.$ui.$newItemInput.focus();
            })

            // Bind CLICK event handler
            todo.$ui.on("click.jqueryTodoList", ".jquery-todolist-action", function () {

                var $this = $(this),
                    tmp1;

                    // SHOW ITEM REMOVE CONFIRM
                if ($this.is(".jquery-todolist-item-action-remove")) {

                    $this
                        .hide()
                        .closest(".jquery-todolist-item-actions-right")
                        .append(
                            $(setup.confirmButtonTemplate)
                                .find(".jquery-todolist-button-confirm")
                                .addClass("ui-state-error jquery-todolist-item-action-remove-confirmed")
                                .html(todo.opt.removeLabel)
                                .end()
                                .find(".jquery-todolist-button-cancel")
                                .addClass("jquery-todolist-item-action-remove-cancel")
                                .end()
                        );

                    // CANCEL DELETE OF ITEM
                } else if ($this.is(".jquery-todolist-item-action-remove-cancel")) {

                    $this
                        .closest(".jquery-todolist-item-actions-right")
                        .find(".jquery-todolist-item-action-remove")
                        .show()
                        .end()
                        .end()
                        .closest(".jquery-todolist-button")
                        .remove();

                    // REMOVE ITEM
                } else if ($this.is(".jquery-todolist-item-action-remove-confirmed")) {

                    $this.closest(".jquery-todolist-item")
                        .slideUp().promise().then(function () {

                            $(this).remove();

                        });

                    $this.trigger("todolist-remove", options, $this);
                    $this.trigger("todolist-change", options, $this);

                    // MARK CHECKED/UNCHECKED
                } else if ($this.is(".jquery-todolist-item-checkbox")) {

                    tmp1 = $this.closest(".jquery-todolist-item");

                    if (tmp1.hasClass("jquery-todolist-item-done")) {

                        tmp1.removeClass("jquery-todolist-item-done");

                        $this.trigger("todolist-unchecked", options, $this);
                        $this.trigger("todolist-change", options, $this);

                    } else {

                        tmp1.addClass("jquery-todolist-item-done");

                        $this.trigger("todolist-checked", options, $this);
                        $this.trigger("todolist-change", options, $this);

                        $this.closest(".jquery-todolist-item").delay(500)
                            .slideUp().promise().then(function () {

                                $(this).remove();

                            });

                        var postData = {
                            ItemKey: $this.closest(".jquery-todolist-item").data('key'),
                            ArcKey: arcKey
                        }

                        ajaxPost(postData, deleteTodoItemAction, function () {})

                    }

                    // SHOW EDIT FORM?
                } else if ($this.is(".jquery-todolist-action-edit")) {

                    todo.showEdit($this);

                    // SHOW THE TODOLIST MENU
                } else if ($this.is(".jquery-todolist-menu-show")) {

                    if (!todo.$ui.$menu) {

                        todo.setupListMenu();

                    }

                    if (todo.$ui.$menu.is(":visible")) {

                        todo.$ui.$menu.hide();

                    } else {

                        todo.$ui.$menu.show();

                    }


                    // SHOW/HIDE DONE ITEMS
                } else if ($this.is(".jquery-todolist-menu-item")) {


                    if ($this.data("todolistaction") === "toggle done") {

                        if (todo.$ui.$items.hasClass("jquery-todolist-hide-done")) {

                            todo.$ui.$items.removeClass("jquery-todolist-hide-done");

                        } else {

                            todo.$ui.$items.addClass("jquery-todolist-hide-done");

                        }

                    } //end: toggle done

                    $this.closest(".jquery-todolist-menu").hide();


                    // CLOSE A MENU
                } else if ($this.data("todolistaction") === "close menu") {

                    $this.closest(".jquery-todolist-menu").hide();

                }

            });

            // Bind add input ENTER key handler
            todo.$ui.$newItemInput.on("keyup", function (ev) {

                if (ev.which === 13) {

                    //todo.$ui.$addButton.click();
                    todo.$ui.$newItemInput.val(todo.$ui.$newItemInput.val().replace(/[\r\n\v]+/g, ''))
                    var tmp1;

                    // ADD NEW ITEM
                    tmp1 = todo.$ui.$newItemInput.val();

                    if (tmp1) {

                        todo.addItem({ title: tmp1 }, false, createGuid());

                    }

                    todo.$ui.$newItemInput.val("");
                    todo.$ui.$newItemInput.hide('fast');

                }

            });


            // Store this instance object on the dom element
            this.jqueryTodoList = todo;
            todo.$ui.css("display", "");

            // Focus on Title?
            if (todo.opt.focusOnTitle) {

                setTimeout(function () {

                    todo.$ui.find("div.jquery-todolist-title")
                        .find(".jquery-todolist-title-text")
                        .click()
                        .end()
                        .find("input")
                        .select();

                }, 500);

            }

            return this;

        }); //end: this.each()

        return returnValue;

    }; //end: $.fn.todolist

}));
