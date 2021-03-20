function initJqgrid(gridId, gridNavId, columnNames, columnModel, dataUrl, editUrl,
    edit, add, del, search, title, showTitle) {
    buildJqGrid(gridId, gridNavId, columnNames, columnModel, dataUrl, editUrl,
    edit, add, del, search, title, showTitle, 'auto');
}

function initFixedJqgrid(gridId, gridNavId, columnNames, columnModel, dataUrl, editUrl,
    edit, add, del, search, title, showTitle, height) {
    buildJqGrid(gridId, gridNavId, columnNames, columnModel, dataUrl, editUrl,
    edit, add, del, search, title, showTitle, height);
}



function buildJqGrid(gridId, gridNavId, columnNames, columnModel, dataUrl, editUrl,
    edit, add, del, search, title, showTitle, height){
jQuery(gridId).jqGrid({
        url: dataUrl,
        datatype: "json",
        colNames: columnNames,
        colModel: columnModel,
        rowNum: 30,
        mtype: 'GET',
        loadonce: true,
        rowList: [30, 60, 90],
        pager: gridNavId,
        sortname: 'Id',
        viewrecords: true,
        sortorder: 'asc',
        caption: title,
        editurl: editUrl,
        width: window.innerWidth * 0.95,
        shrinkToFit: true,
        height: height ,
        direction:"rtl"
    });
    if (!showTitle) {
        $(".ui-jqgrid-titlebar").hide();
    }
    $(gridId).jqGrid('navGrid', gridNavId,
               {
                   edit: edit,
                   add: add,
                   del: del,
                   search: search,
                   searchtext: "بحث",
                   addtext: "إضافة",
                   edittext: "تعديل",
                   deltext: "حذف"
               },
               {   //EDIT
                   //                       height: 300,
                   //                       width: 400,
                   //                       top: 50,
                   //                       left: 100,
                   //                       dataheight: 280,
                   modal: true,
                   closeOnEscape: true,//Closes the popup on pressing escape key
                   closeAfterEdit: true,
                   reloadAfterSubmit: true,
                   recreateForm: true,
                   drag: true,
                   afterSubmit: function (response, postdata) {
                       if (response.responseText == "") {

                           $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');//Reloads the grid after edit
                           return [true, '']
                       }
                       else {
                           //$(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); //Reloads the grid after edit
                           return [false, response.responseText]//Captures and displays the response text on th Edit window
                       }
                   },
                   beforeShowForm: function (form) {

                       beforShowJqGridDialog(gridId);
                   },
                   editData: {
                       ItemId: function () {
                           var sel_id = $(gridId).jqGrid('getGridParam', 'selrow');
                           var value = $(gridId).jqGrid('getCell', sel_id, 'Id');
                           return value;
                       }
                   }
               },
               {
                   modal: true,
                   closeAfterAdd: true,//Closes the add window after add
                   recreateForm: true,
                   afterSubmit: function (response, postdata) {
                       if (response.responseText == "") {

                           $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')//Reloads the grid after Add
                           return [true, '']
                       }
                       else {
                           //$(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')//Reloads the grid after Add
                           return [false, response.responseText]
                       }
                   }
                   , beforeShowForm: function (form) {

                       beforShowJqGridDialog(gridId);
                   },
               },
               {   //DELETE
                   closeOnEscape: true,
                   closeAfterDelete: true,
                   reloadAfterSubmit: true,
                   drag: true,
                   afterSubmit: function (response, postdata) {
                       if (response.responseText == "") {

                           $(gridId).trigger("reloadGrid", [{ current: true }]);
                           return [true, response.responseText]
                       }
                       else {
                           $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')
                           return [false, response.responseText]
                       }
                   },
                   delData: {

                       ItemId: function () {
                           var sel_id = $(gridId).jqGrid('getGridParam', 'selrow');
                           var value = $(gridId).jqGrid('getCell', sel_id, 'Id');
                           return value;
                       }
                   }
               },
               {//SEARCH
                   closeOnEscape: true

               }
        );
}

function initPagedJqgrid(gridId, gridNavId, columnNames, columnModel, dataUrl, editUrl,
    edit, add, del, search, title, showTitle) {
    jQuery(gridId).jqGrid({
        url: dataUrl,
        datatype: 'json',
        colNames: columnNames,
        colModel: columnModel,
        rowNum: 30,
        rowList: [5,10, 20, 30, 50],
        mtype: "GET",
        //This property is very usefull.
        loadonce: false,
        pager: gridNavId,
        sortname: 'Id',
        viewrecords: true,
        sortorder: 'asc',
        sortable: true,
        caption: title,
        editurl: editUrl,
        width: window.innerWidth * 0.95,
        shrinkToFit: true,
        height: 'auto',
        direction:"rtl",
        onPaging: function (b) {
            var nextPg = $(this).getGridParam("page");
            currPg = nextPg;
            return;
        }
    });
    if (!showTitle) {
        $(".ui-jqgrid-titlebar").hide();
    }
    $(gridId).jqGrid('navGrid', gridNavId,
               {
                   edit: edit,
                   add: add,
                   del: del,
                   search: search,
                   searchtext: "بحث",
                   addtext: "إضافة",
                   edittext: "تعديل",
                   deltext: "حذف"
               },
               {   //EDIT
                   //                       height: 300,
                   //                       width: 400,
                   //                       top: 50,
                   //                       left: 100,
                   //                       dataheight: 280,
                   modal: true,
                   closeOnEscape: true,//Closes the popup on pressing escape key
                   closeAfterEdit: true,
                   reloadAfterSubmit: true,
                   recreateForm: true,
                   drag: true,
                   afterSubmit: function (response, postdata) {
                       if (response.responseText == "") {

                           $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');//Reloads the grid after edit
                           return [true, '']
                       }
                       else {
                           //$(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); //Reloads the grid after edit
                           return [false, response.responseText]//Captures and displays the response text on th Edit window
                       }
                   },
                   beforeShowForm: function (form) {

                       beforShowJqGridDialog(gridId);
                   },
                   editData: {
                       ItemId: function () {
                           var sel_id = $(gridId).jqGrid('getGridParam', 'selrow');
                           var value = $(gridId).jqGrid('getCell', sel_id, 'Id');
                           return value;
                       }
                   }
               },
               {
                   modal: true,
                   closeAfterAdd: true,//Closes the add window after add
                   recreateForm: true,
                   afterSubmit: function (response, postdata) {
                       if (response.responseText == "") {

                           $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')//Reloads the grid after Add
                           return [true, '']
                       }
                       else {
                           //$(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')//Reloads the grid after Add
                           return [false, response.responseText]
                       }
                   }
                   , beforeShowForm: function (form) {

                       beforShowJqGridDialog(gridId);
                   },
               },
               {   //DELETE
                   closeOnEscape: true,
                   closeAfterDelete: true,
                   reloadAfterSubmit: true,
                   drag: true,
                   afterSubmit: function (response, postdata) {
                       if (response.responseText == "") {

                           $(gridId).trigger("reloadGrid", [{ current: true }]);
                           return [true, response.responseText]
                       }
                       else {
                           $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')
                           return [false, response.responseText]
                       }
                   },
                   delData: {

                       ItemId: function () {
                           var sel_id = $(gridId).jqGrid('getGridParam', 'selrow');
                           var value = $(gridId).jqGrid('getCell', sel_id, 'ID');
                           return value;
                       }
                   }
               },
               {//SEARCH
                   closeOnEscape: true

               }
        );
}


function initJqgridWithSubGrid(gridId, gridNavId, columnNames, columnModel, dataUrl, editUrl,
    edit, add, del, search, title, showTitle, subGridUrl, subGridColModel, subGridModel) {
    jQuery(gridId).jqGrid({
        url: dataUrl,
        datatype: "json",
        colNames: columnNames,
        colModel: columnModel,
        rowNum: 30,
        mtype: 'GET',
        loadonce: true,
        rowList: [30, 60, 90],
        pager: gridNavId,
        sortname: 'Id',
        viewrecords: true,
        sortorder: 'asc',
        caption: title,
        editurl: editUrl,
        width: window.innerWidth * 0.95,
        shrinkToFit: true,
        height: 'auto',
        subGrid: true,
        direction:"rtl",
        subGridRowExpanded: function (subgrid_id, row_id) {
            // we pass two parameters
            // subgrid_id is a id of the div tag created within a table
            // the row_id is the id of the row
            // If we want to pass additional parameters to the url we can use
            // the method getRowData(row_id) - which returns associative array in type name-value
            // here we can easy construct the following
            var subgrid_table_id;
            var rowData = $(this).getRowData(row_id);
    		var uid = rowData['Id'];
            var handlerUrl = subGridUrl + "?ID=" + uid;
            subgrid_table_id = subgrid_id + "_t";
            jQuery("#" + subgrid_id).html("<table id='" + subgrid_table_id + "' class='scroll'></table>");
            jQuery("#" + subgrid_table_id).jqGrid({
                url: handlerUrl,
                datatype: "json",
                colNames: subGridColModel,
                colModel: subGridModel,
                height: '100%',
                loadonce: true,
                mtype: 'GET',
                rowNum: 20,
                sortname: 'num',
                sortorder: "asc",
                direction:"rtl"
            });
        }
    });
    if (!showTitle) {
        $(".ui-jqgrid-titlebar").hide();
    }
    $(gridId).jqGrid('navGrid', gridNavId,
               {
                   edit: edit,
                   add: add,
                   del: del,
                   search: search,
                   searchtext: "بحث",
                   addtext: "إضافة",
                   edittext: "تعديل",
                   deltext: "حذف"
               },
               {   //EDIT
                   //                       height: 300,
                   //                       width: 400,
                   //                       top: 50,
                   //                       left: 100,
                   //                       dataheight: 280,
                   modal: true,
                   closeOnEscape: true,//Closes the popup on pressing escape key
                   closeAfterEdit: true,
                   reloadAfterSubmit: true,
                   drag: true,
                   recreateForm: true,
                   afterSubmit: function (response, postdata) {
                       if (response.responseText == "") {

                           $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');//Reloads the grid after edit
                           return [true, '']
                       }
                       else {
                           //$(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); //Reloads the grid after edit
                           return [false, response.responseText]//Captures and displays the response text on th Edit window
                       }
                   },
                   beforeShowForm: function (form) {

                       beforShowJqGridDialog(gridId);
                   },
                   editData: {
                       ItemId: function () {
                           var sel_id = $(gridId).jqGrid('getGridParam', 'selrow');
                           var value = $(gridId).jqGrid('getCell', sel_id, 'Id');
                           return value;
                       }
                   }
               },
               {
                   modal: true,
                   closeAfterAdd: true,//Closes the add window after add
                   recreateForm: true,
                   afterSubmit: function (response, postdata) {
                       if (response.responseText == "") {

                           $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')//Reloads the grid after Add
                           return [true, '']
                       }
                       else {
                           //$(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')//Reloads the grid after Add
                           return [false, response.responseText]
                       }
                   }
                   , beforeShowForm: function (form) {

                       beforShowJqGridDialog(gridId);
                   },
               },
               {   //DELETE
                   closeOnEscape: true,
                   closeAfterDelete: true,
                   reloadAfterSubmit: true,
                   drag: true,
                   afterSubmit: function (response, postdata) {
                       if (response.responseText == "") {

                           $(gridId).trigger("reloadGrid", [{ current: true }]);
                           return [true, response.responseText]
                       }
                       else {
                           $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')
                           return [false, response.responseText]
                       }
                   },
                   delData: {

                       ItemId: function () {
                           var sel_id = $(gridId).jqGrid('getGridParam', 'selrow');
                           var value = $(gridId).jqGrid('getCell', sel_id, 'ID');
                           return value;
                       }
                   }
               },
               {//SEARCH
                   closeOnEscape: true

               }
        );
}

function resizeJqGridWidth(grid_id, div_id, width) {
    $(window).bind('resize', function () {
        $('#' + grid_id).setGridWidth(width, true); //Back to original width
        $('#' + grid_id).setGridWidth($('#' + div_id).width(), true); //Resized to new width as per window
         $('#' + grid_id + '_left').children(":first").css('FLOAT', 'right');
        $('#' + grid_id + 'Pager_left').css('Width', '30%');
    }).trigger('resize');
}


function beforShowJqGridDialog(gridId) {
    var gridName = gridId.substr(1, gridId.length);

    var $infoDlg = $("#editmod" + gridName);
    var $parentDiv = $infoDlg.parent();
    //$infoDlg[0].style.width = Math.round(parentWidth * 0.6) + "px";
    var dlgWidth = $infoDlg.width()
    var parentWidth = $(window).innerWidth();
    var dlgHeight = $infoDlg.height();
    var parentHeight = $(window).innerHeight();


    $infoDlg[0].style.left = Math.round((parentWidth / 2) - (dlgWidth / 2)) + "px";
    $infoDlg[0].style.top = Math.round((parentHeight / 2) - ((dlgHeight / 2) - 30)) + "px";


    //sorting dropdown
    $("select").each(function () {
        var selectID = $(this).attr('id');
        var my_options = $("#" + selectID + " option");
        var value = $("select#" + selectID + " option:selected").val();
        my_options.sort(function (a, b) {
            if (a.text > b.text) {
                //alert(a.text);
                return 1;
            }
            else if (a.text < b.text) return -1;
            else return 0
        });
        $("#" + selectID).empty().append(my_options);

        $("#" + selectID).val(value);
    });
}

function beforShowJqGridDialogWithParam(gridId, height, width) {
    var gridName = gridId.substr(1, gridId.length);

    var $infoDlg = $("#editmod" + gridName);
    var $parentDiv = $infoDlg.parent();
    $infoDlg[0].style.width = width + "px";
    $infoDlg[0].style.height = height + "px";
    var dlgWidth = $infoDlg.width()
    var parentWidth = $(window).innerWidth();
    var dlgHeight = $infoDlg.height();
    var parentHeight = $(window).innerHeight();


    $infoDlg[0].style.left = Math.round((parentWidth / 2) - (dlgWidth / 2)) + "px";
    $infoDlg[0].style.top = Math.round((parentHeight / 2) - ((dlgHeight / 2) - 30)) + "px";


    //sorting dropdown
    $("select").each(function () {
        var selectID = $(this).attr('id');
        var my_options = $("#" + selectID + " option");
        var value = $("select#" + selectID + " option:selected").val();
        my_options.sort(function (a, b) {
            if (a.text > b.text) {
                //alert(a.text);
                return 1;
            }
            else if (a.text < b.text) return -1;
            else return 0
        });
        $("#" + selectID).empty().append(my_options);

        $("#" + selectID).val(value);
    });
}

function initClientJqgrid(gridId, gridNavId, columnNames, columnModel, edit,
    add, del, search, title, showTitle, jsonData, _target, _targetCounter) {
    var lastSel = _targetCounter.val();
    onclickSubmitLocal = function (options, postdata) {
        var gId=options.gbox.replace("gbox_","");
        var currentGrid = $(gId);
        var grid_p = currentGrid[0].p,
            idname = grid_p.prmNames.id,
            grid_id = currentGrid[0].id,
            id_in_postdata = grid_id + "_id",
            rowid = postdata[id_in_postdata],
            addMode = rowid === "_empty",
            oldValueOfSortColumn;

        // postdata has row id property with another name. we fix it:
        if (addMode) {
            // generate new id
            var new_id = grid_p.records + 1;
            while ($("#" + new_id).length !== 0) {
                new_id++;
            }
            postdata[idname] = String(new_id);
        } else if (typeof (postdata[idname]) === "undefined") {
            // set id property only if the property not exist
            postdata[idname] = rowid;
        }
        delete postdata[id_in_postdata];

        // prepare postdata for tree grid
        if (grid_p.treeGrid === true) {
            if (addMode) {
                var tr_par_id = grid_p.treeGridModel === 'adjacency' ? grid_p.treeReader.parent_id_field : 'parent_id';
                postdata[tr_par_id] = grid_p.selrow;
            }

            $.each(grid_p.treeReader, function (i) {
                if (postdata.hasOwnProperty(this)) {
                    delete postdata[this];
                }
            });
        }

        // decode data if there encoded with autoencode
        if (grid_p.autoencode) {
            $.each(postdata, function (n, v) {
                postdata[n] = $.jgrid.htmlDecode(v); // TODO: some columns could be skipped
            });
        }

        // save old value from the sorted column
        oldValueOfSortColumn = grid_p.sortname === "" ? undefined : currentGrid.jqGrid('getCell', rowid, grid_p.sortname);

        // save the data in the grid
        if (grid_p.treeGrid === true) {
            if (addMode) {
                currentGrid.jqGrid("addChildNode", rowid, grid_p.selrow, postdata);
            } else {
                currentGrid.jqGrid("setTreeRow", rowid, postdata);
            }
        } else {
            if (addMode) {
                currentGrid.jqGrid("addRowData", postdata[idname], postdata, options.addedrow);
            } else {
                currentGrid.jqGrid("setRowData", rowid, postdata);
            }
        }

        if ((addMode && options.closeAfterAdd) || (!addMode && options.closeAfterEdit)) {
            // close the edit/add dialog
            $.jgrid.hideModal("#editmod" + grid_id,
                              { gb: "#gbox_" + grid_id, jqm: options.jqModal, onClose: options.onClose });
        }

        if (postdata[grid_p.sortname] !== oldValueOfSortColumn) {
            // if the data are changed in the column by which are currently sorted
            // we need resort the grid
            setTimeout(function () {
                currentGrid.trigger("reloadGrid", [{ current: true }]);
            }, 100);
        }

        // !!! the most important step: skip ajax request to the server
        this.processing = true;
        var targetColumn;
        if (gId == "#grdReference")
        {
            targetColumn = "#FinanceRefrenceJson";
        }
        if (gId == "#grdTax")
        {
            targetColumn = "#FinanceOperationTaxJson";
        }
        if (gId == "#grdd") {
            targetColumn = "#FinanceOperationItemString";
        }
        $(targetColumn).val(JSON.stringify(currentGrid.jqGrid('getGridParam', 'data')));
        return {};
    },
    editSettings = {
        //recreateForm:true,
        jqModal: true,
        reloadAfterSubmit: false,
        closeOnEscape: true,
        savekey: [true, 13],
        closeAfterEdit: true,
        recreateForm: true,
        onclickSubmit: onclickSubmitLocal,
        modal: true
    },
    addSettings = {
        //recreateForm:true,
        jqModal: true,
        reloadAfterSubmit: false,
        savekey: [true, 13],
        closeOnEscape: true,
        closeAfterAdd: true,
        recreateForm: true,
        onclickSubmit: onclickSubmitLocal,
        modal:true
    },
    delSettings = {
        // because I use "local" data I don't want to send the changes to the server
        // so I use "processing:true" setting and delete the row manually in onclickSubmit
        onclickSubmit: function (options, rowid) {
            var currentGrid = $(gridId);
            var grid_id = $.jgrid.jqID(currentGrid[0].id),
                grid_p = currentGrid[0].p,
                newPage = currentGrid[0].p.page;

            // delete the row
            $(gridId).delRowData(rowid);
            $.jgrid.hideModal("#delmod" + grid_id,
                              { gb: "#gbox_" + grid_id, jqm: options.jqModal, onClose: options.onClose });

            if (grid_p.lastpage > 1) {// on the multipage grid reload the grid
                if (grid_p.reccount === 0 && newPage === grid_p.lastpage) {
                    // if after deliting there are no rows on the current page
                    // which is the last page of the grid
                    newPage--; // go to the previous page
                }
                // reload grid to make the row from the next page visable.
                $(gridId).trigger("reloadGrid", [{ page: newPage }]);
            }
            _target.val(JSON.stringify($(gridId).jqGrid('getGridParam', 'data')));
            return true;
        },
        processing: true
    },

$(gridId).jqGrid({
    datatype: 'local',
    data: jsonData,
    colNames: columnNames,
    colModel: columnModel,
    rowNum: 4,
    rowList: [4],
    pager: gridNavId,
    gridview: true,
    rownumbers: true,
    autoencode: true,
    ignoreCase: true,
    sortname: 'id',
    viewrecords: true,
    sortorder: 'desc',
    caption: title,
    height: 75,
    width: 500,
    editurl: 'clientArray',
    direction:"rtl",
    ondblClickRow: function (rowid, ri, ci) {
        var p = $(gridId)[0].p;
        if (p.selrow !== rowid) {
            // prevent the row from be unselected on double-click
            // the implementation is for "multiselect:false" which we use,
            // but one can easy modify the code for "multiselect:true"
            $(gridId).jqGrid('setSelection', rowid);
        }
        $(gridId).jqGrid('editGridRow', rowid, editSettings);
    },
    onSelectRow: function (id) {
        var lastSel = _targetCounter.val();
        if (id && id !== lastSel) {
            // cancel editing of the previous selected row if it was in editing state.
            // jqGrid hold intern savedRow array inside of jqGrid object,
            // so it is safe to call restoreRow method with any id parameter
            // if jqGrid not in editing state
            if (typeof lastSel !== "undefined") {
                $(gridId).jqGrid('restoreRow', lastSel);
            }
            lastSel = id;
            _targetCounter.val(id);
        }
    }
}).jqGrid('navGrid', gridNavId, { search: false }, editSettings, addSettings, delSettings,
  {
      //search: false, overlay: false,
      //onClose: function (form) {
      //    // if we close the search dialog during the datapicker are opened
      //    // the datepicker will stay opened. To fix this we have to hide
      //    // the div used by datepicker
      //    $("div#ui-datepicker-div.ui-datepicker").hide();
      //}
  });
}

function initJqTree(gridId, gridNavId, columnNames, columnModel, expandColumn, dataUrl, editUrl,
    edit, add, del, search, title, showTitle) {
    var listParameter = '';
    jQuery(gridId).jqGrid({
        url: dataUrl,
        datatype: "json",
        colNames: columnNames,
        colModel: columnModel,
        mtype: 'GET',
        pager: gridNavId,
        viewrecords: false,
        caption: title,
        shrinkToFit: true,
        autowidth: true,
        height: 'auto',
        direction: "rtl",
        ExpandColumn: expandColumn,
        treeGrid:true,
        treedatatype:'json',
        treeGridModel: 'adjacency',
        treeReader: {
            parent_id_field: 'ParentId',
            level_field: 'Level',
            leaf_field: 'IsLeaf',
            expanded_field: 'Expanded',
            loaded: 'Loaded',
            icon_field: 'icon'
        }
    });
     if (!showTitle) {
        $(".ui-jqgrid-titlebar").hide();
    }
    $(gridId).jqGrid('navGrid', gridNavId,
               {
                   edit: edit,
                   add: add,
                   del: del,
                   search: search,
                   searchtext: "بحث",
                   addtext: "إضافة",
                   edittext: "تعديل",
                   deltext: "حذف"
               },
               {   //EDIT
                   //                       height: 300,
                   //                       width: 400,
                   //                       top: 50,
                   //                       left: 100,
                   //                       dataheight: 280,
                   modal: true,
                   closeOnEscape: true,//Closes the popup on pressing escape key
                   closeAfterEdit: true,
                   reloadAfterSubmit: true,
                   recreateForm: true,
                   drag: true,
                   afterSubmit: function (response, postdata) {
                       if (response.responseText == "") {

                           $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');//Reloads the grid after edit
                           return [true, '']
                       }
                       else {
                           //$(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); //Reloads the grid after edit
                           return [false, response.responseText]//Captures and displays the response text on th Edit window
                       }
                   },
                   beforeShowForm: function (form) {

                       beforShowJqGridDialog(gridId);
                   },
                   editData: {
                       ItemId: function () {
                           var sel_id = $(gridId).jqGrid('getGridParam', 'selrow');
                           var value = $(gridId).jqGrid('getCell', sel_id, 'Id');
                           return value;
                       }
                   }
               },
               {
                   modal: true,
                   closeAfterAdd: true,//Closes the add window after add
                   recreateForm: true,
                   afterSubmit: function (response, postdata) {
                       if (response.responseText == "") {

                           $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')//Reloads the grid after Add
                           return [true, '']
                       }
                       else {
                           //$(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')//Reloads the grid after Add
                           return [false, response.responseText]
                       }
                   }
                   , beforeShowForm: function (form) {

                       beforShowJqGridDialog(gridId);
                   },
               },
               {   //DELETE
                   closeOnEscape: true,
                   closeAfterDelete: true,
                   reloadAfterSubmit: true,
                   drag: true,
                   afterSubmit: function (response, postdata) {
                       if (response.responseText == "") {

                           $(gridId).trigger("reloadGrid", [{ current: true }]);
                           return [true, response.responseText]
                       }
                       else {
                           $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')
                           return [false, response.responseText]
                       }
                   },
                   delData: {

                       ItemId: function () {
                           var sel_id = $(gridId).jqGrid('getGridParam', 'selrow');
                           var value = $(gridId).jqGrid('getCell', sel_id, 'Id');
                           return value;
                       }
                   }
               },
               {//SEARCH
                   closeOnEscape: true

               }
        );
}




function initCustomjqGrid(gridId, gridNavId, columnNames, columnModel, dataUrl, editUrl,
    edit, add, del, search, title, showTitle, viewPaging
    ) {
    jQuery(gridId).jqGrid({
        url: dataUrl,
        datatype: "json",
        colNames: columnNames,
        colModel: columnModel,
        rowNum: 30,
        mtype: 'GET',
        loadonce: true,
        rowList: (viewPaging == true ? [30, 60, 90] : []),
        pager: gridNavId,
        sortname: 'Id',
        viewrecords: viewPaging,
        pgbuttons: viewPaging,
        pgtext: (viewPaging == true ? 'Page' : null),
        sortorder: 'asc',
        caption: title,
        editurl: editUrl,
        //width:window.innerWidth * 0.95,
        shrinkToFit: true,
        height: 'auto',
        direction: "rtl",
        onSelectRow: function (id, status) {
            try {
                if (Jqgrid_RowSelected != null)
                    Jqgrid_RowSelected(gridId, id, status);
            } catch (ex) {

            }
        }
    });
    if (!showTitle) {
        $(".ui-jqgrid-titlebar").hide();
    }
    $(gridId).jqGrid('navGrid', gridNavId,
               {
                   edit: edit,
                   add: add,
                   del: del,
                   search: search,
                   searchtext: "Search",
                   addtext: "Add new",
                   edittext: "Edit",
                   deltext: "Delete"
               },
               {   //EDIT
                   //                       height: 300,
                   width: 450,
                   //                       top: 50,
                   //                       left: 100,
                   //                       dataheight: 280,
                   modal: true,
                   closeOnEscape: true,//Closes the popup on pressing escape key
                   closeAfterSearch: true,
                   closeAfterEdit: true,
                   reloadAfterSubmit: true,
                   recreateForm: true,
                   drag: true,
                   afterSubmit: function (response, postdata) {
                       if (response.responseText == "") {

                           $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid');//Reloads the grid after edit
                           return [true, '']
                       }
                       else {
                           //$(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid'); //Reloads the grid after edit
                           return [false, response.responseText]//Captures and displays the response text on th Edit window
                       }
                   },
                   beforeShowForm: function (form) {

                       beforShowJqGridDialog(gridId);
                   },
                   editData: {
                       ItemId: function () {
                           var sel_id = $(gridId).jqGrid('getGridParam', 'selrow');
                           var value = $(gridId).jqGrid('getCell', sel_id, 'Id');
                           return value;
                       }
                   }
               },
               {
                   modal: true,
                   closeAfterAdd: true,//Closes the add window after add
                   recreateForm: true,
                   afterSubmit: function (response, postdata) {
                       if (response.responseText == "") {

                           $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')//Reloads the grid after Add
                           return [true, '']
                       }
                       else {
                           //$(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')//Reloads the grid after Add
                           return [false, response.responseText]
                       }
                   }
                   , beforeShowForm: function (form) {

                       beforShowJqGridDialog(gridId);
                   },
               },
               {   //DELETE
                   closeOnEscape: true,
                   closeAfterDelete: true,
                   reloadAfterSubmit: true,
                   drag: true,
                   afterSubmit: function (response, postdata) {
                       if (response.responseText == "") {

                           $(gridId).trigger("reloadGrid", [{ current: true }]);
                           return [true, response.responseText]
                       }
                       else {
                           $(this).jqGrid('setGridParam', { datatype: 'json' }).trigger('reloadGrid')
                           return [false, response.responseText]
                       }
                   },
                   delData: {

                       ItemId: function () {
                           var sel_id = $(gridId).jqGrid('getGridParam', 'selrow');
                           var value = $(gridId).jqGrid('getCell', sel_id, 'ID');
                           return value;
                       }
                   }
               },
               {//SEARCH
                   closeOnEscape: true

               }
        );
}