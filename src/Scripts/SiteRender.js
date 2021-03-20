var RoleDetailesPage = function () {
    $.ajax({
        url: "/Roles/DetailsJson/" + $("#urlId").val(),
        type: 'Get',
    dataType: 'json',
    contentType: 'application/json',
    beforeSend: function() {
        $(".loader").removeClass('hidden');
    },
    complete: function() {
        $(".loader").addClass('hidden');
    },
    success: function (data) {

        var addDashedString = function (str) {
            if (str === null || str.length === 0 || str === " ")
                return "";
            return "  (" + str + ") ";
        };


        $("#id").text(data.id);
        $("#applicationName").text(data.applicationName);
        $("#roleName").text(data.roleName);
        $("#description").text(data.description);

        $("#addUser").attr("action", "/Roles/AddUser/" + data.id);
        $("#addOperation").attr("action", "/Roles/AddOperation/" + data.id);
        $("#addView").attr("action", "/Roles/AddView/" + data.id);

        if (0 < data.otherUsers.length) {
            var addUserSelect = $("#addUserSelect");
            $.each(data.otherUsers,
                function(index, elem) {
                    addUserSelect.append("<option value='" + elem.userName + "'>" + elem.userName + "</option>");
                });
        }

        if (0 < data.otherOperations.length) {
            $("#addOperation").attr("action", "/Roles/AddOperation/" + data.id);
            var addOperationSelect = $("#addOperationSelect");
            $.each(data.otherOperations,
                function(index, elem) {
                    addOperationSelect.append("<option value='" + elem.id + "'>" + elem.name + "</option>");
                });
        }

        if (0 < data.otherViews.length) {
            var addViewSelect = $("#addViewSelect");
            $.each(data.otherViews, function (index, elem) {
                addViewSelect.append("<option value='" + elem.id + "'>" + elem.name + addDashedString(elem.sectionName) + addDashedString(elem.url)
                    + "</option>");
            });
        }


        var roleUsers = $(".roleUsers");
        if (0 === data.users.length) {
            roleUsers.html("<p> <i> No User Assigned </i> </p>");
        } else {
            $.each(data.users,
                function(index, elem) {
                    roleUsers.append(
                        "<div class='hoverable'>" + elem.userName + addDashedString(elem.firstName + " " + elem.lastName) +
                        "<a class='btn btn-xs btn-danger pull-right deleteButton' href='/Roles/RemoveUser/" + data.id + "?userName=" + elem.userName +
                        "'> delete </a>" +
                        "<a class='btn btn-xs btn-primary pull-right' href='/Users/Details/" + elem.id + "'> details</a></div>");
                });
        }


        var roleOperations = $(".roleOperations");
        if (0 === data.operations.length)
        {
            roleOperations.html("<p> <i> No Operation Assigned </i> </p>");
        }
        else
        {
            $.each(data.operations,
                function(index, elem) {
                    roleOperations.append("<div class='hoverable'>" + elem.name + addDashedString(elem.description) +
                        "<a class='btn btn-xs btn-danger pull-right deleteButton' href='/Roles/RemoveOperation/" + data.id +
                        "?operationId=" + elem.id + "'> delete </a> </div>");
                });
        }


        var roleViews = $(".roleViews");
        if (0 === data.views.length)
        {
            roleViews.html("<p> <i> No Views Assigned </i> </p>");
        }
        else
        {
            $.each(data.views, function (index, elem)
            {
                roleViews.append("<div class='hoverable'>" + elem.name + addDashedString(elem.sectionName) + addDashedString(elem.url) +
                        "<a class='btn btn-xs btn-danger pull-right deleteButton' href='/Roles/RemoveView/" +
                        data.id + "?viewId=" + elem.id + "'> delete </a> </div>");
            });
        }



        $(".deleteButton").on("click", function (e) {
            var link = this;
            e.preventDefault();

            $("<div>Are you sure you want to delete?</div>").dialog({
                buttons: {
                    "Ok": function () { window.location = link.href; },
                    "Cancel": function () { $(this).dialog("close"); }
                }
            });
        });
    },
    error: function (request, status, error) { alert(request.responseText); }
});

$(".TabPageLinks").on("click",
    function openTab() {
        $(".TabPageContent").each(function() { $(this).css("display", "none"); });
        $(".TabPageLinks").each(function() { $(this).removeClass("active"); });

        var btn = $(this);
        $("#" + btn.data("tabname") + "").css("display", "block");
        btn.addClass(" active");

        if (history.pushState) {
            var newurl = window.location.protocol + "//" + window.location.host + window.location.pathname + '?tab=' + btn.data("tab");
            window.history.pushState({ path: newurl }, '', newurl);
        }
    });

$("#" + $("#urlTapPage").val() + "").click();
};

var RoleIndexPage = function() {

        var jqGridId = "#jqGrid";
        var jqGridPagerId = "#jqGridPager";

        var jqGridColName = ["المعرف", "اسم", "معرف التطبيق", "اسم التطبيق", "الوصف"];
        var jqGridColModel = [
            { key: false, name: "id", index: "id", editable: false, width: 6 },
            { key: false, name: "name", index: "name", editable: false, width: 40 },
            { key: false, name: "applicationId", index: "applicationId", editable: false, width: 40 },
            { key: false, name: "applicationName", index: "applicationName", editable: false, width: 40 },
            { key: false, name: "description", index: "description", editable: false, width: 40 }
        ];
        var jqGridEdit = false;
        var jqGridAdd = false;
        var jqGridDelete = false;
        var jqGridSearch = true;
        var jqGridTitle = "إدارة السماحيات";
        var jqGridShowTitle = "";

        initJqgrid(jqGridId,
            jqGridPagerId,
            jqGridColName,
            jqGridColModel,
            "/Roles/AllRoles/",
            '',
            jqGridEdit,
            jqGridAdd,
            jqGridDelete,
            jqGridSearch,
            jqGridTitle,
            jqGridShowTitle);


        jQuery("#jqGrid").navButtonAdd("#jqGridPager",
            {
                caption: "تفاصيل",
                title: "تفاصيل",
                buttonicon: "circle-check",
                onClickButton: function() {
                    var selectedRowId = jQuery("#jqGrid").jqGrid("getGridParam", "selrow");
                    if (!selectedRowId)
                        return;

                    var selectedRow = jQuery("#jqGrid").jqGrid("getRowData", selectedRowId);
                    var url = selectedRow['id'];
                    if (url)
                        window.location.href = "/roles/details/" + url;
                },
                position: "last"
            });



    jQuery("#jqGrid").navButtonAdd("#jqGridPager",
        {
            caption: "تعديل",
            title: "تعديل",
            buttonicon: "ui-icon ui-icon-wrench",
            onClickButton: function () {

                var selectedRowId = jQuery("#jqGrid").jqGrid("getGridParam", "selrow");
                if (!selectedRowId)
                    return;

                var selectedRow = jQuery("#jqGrid").jqGrid("getRowData", selectedRowId);
                var url = selectedRow['id'];
                if (url)
                    window.location.href = "/roles/edit/" + url;
            },
            position: "last"
        });
        jQuery("#jqGrid").navButtonAdd("#jqGridPager",
            {
                caption: "حذف",
                title: "حذف",
                buttonicon: "ui-icon ui-icon-trash",
                onClickButton: function() {

                    var selectedRowId = jQuery("#jqGrid").jqGrid("getGridParam", "selrow");
                    if (!selectedRowId)
                        return;

                    var selectedRow = jQuery("#jqGrid").jqGrid("getRowData", selectedRowId);
                    var url = selectedRow['id'];
                    if (url)
                        window.location.href = "/roles/delete/" + url;
                },
                position: "last"
            });
        jQuery("#jqGrid").navButtonAdd("#jqGridPager",
            {
                caption: "جديد",
                title: "جديد",
                //buttonicon: "ui-icon ui-icon-trash",
                onClickButton: function () {
                    window.location.href = "/roles/create/";
                },
                position: "last"
            });

        resizeJqGridWidth('jqGrid', 'divMain', '80%');
};

var RoleDeletePage = function() {
    $.ajax({
        url: "/Roles/DetailsJson/" + $("#urlId").val(),
        type: 'Get',
        dataType: 'json',
        contentType: 'application/json',
        beforeSend: function() {
            $(".loader").removeClass('hidden');
        },
        complete: function() {
            $(".loader").addClass('hidden');
        },
        success: function(data) {
            $("#id").text(data.id);
            $("#applicationName").text(data.applicationName);
            $("#roleName").text(data.roleName);
            $("#description").text(data.description);

            if (0 === data.users.length) {
                $(".roleUsers").html("<p> <i> No User Assigned </i> </p>");
            } else {
                $.each(data.users,
                    function(index, elem) {
                        $(".roleUsers").append(
                            "<div class='hoverable'>" +
                            elem.userName +
                            " - " +
                            elem.firstName +
                            " " +
                            elem.lastName +
                            "<a class='btn btn-xs btn-primary pull-right' href='/Users/Details/" +
                            elem.id +
                            "'> details</a></div>");
                    });
            }

            if (0 === data.operations.length) {
                $(".roleOperations").html("<p> <i> No Operation Assigned </i> </p>");
            } else {
                $.each(data.operations,
                    function(index, elem) {
                        $(".roleOperations")
                            .append("<div class='hoverable'>" + elem.name + " - " + elem.description + "</div>");
                    });
            }

            if (0 === data.operations.length && 0 === data.users.length) {
                $("#msgConfirm").removeClass("hidden");
                $("#deleteBtn").on("click",
                    function() {
                        $.ajax({
                            url: "/Roles/Delete/" + data.id,
                            type: 'Post',
                            success: function(data) {
                                window.location = "/Roles/Index";
                            },
                            error: function(request, status, error) {
                                alert(request.responseText);
                            }
                        });
                    });
            } else {
                $("#msgCantDelete").removeClass("hidden");
                $("#editBtn").attr("href", "/Roles/Details/" + data.id);
            }
        },
        error: function(request, status, error) {
            alert(request.responseText);
        }
    });
};


var UserIndexPage = function() {

    var jqGridId = "#jqGrid";
    var jqGridPagerId = "#jqGridPager";

    var jqGridColName = ["المعرف", "اسم المستخدم", "الاسم الاول", "الكنية"];
    var jqGridColModel = [
        { key: false, name: "id", index: "id", editable: false, width: 6 },
        { key: false, name: "userName", index: "userName", editable: false, width: 40 },
        { key: false, name: "firstName", index: "firstName", editable: false, width: 40 },
        { key: false, name: "lastName", index: "lastName", editable: false, width: 40 }
    ];
    var jqGridEdit = false;
    var jqGridAdd = false;
    var jqGridDelete = false;
    var jqGridSearch = true;
    var jqGridTitle = "إدارة المستخدمين";
    var jqGridShowTitle = "";

    initJqgrid(jqGridId,
        jqGridPagerId,
        jqGridColName,
        jqGridColModel,
        "/Users/AllUsers",
        '',
        jqGridEdit,
        jqGridAdd,
        jqGridDelete,
        jqGridSearch,
        jqGridTitle,
        jqGridShowTitle);


    jQuery("#jqGrid").navButtonAdd("#jqGridPager", {
        caption: "تفاصيل",
        title: "تفاصيل",
        buttonicon: "circle-check",
        onClickButton: function () {
            var selectedRowId = jQuery("#jqGrid").jqGrid("getGridParam", "selrow");
            if (!selectedRowId)
                return;

            var selectedRow = jQuery("#jqGrid").jqGrid("getRowData", selectedRowId);
            var url = selectedRow['id'];
            if (url)
                window.location.href = "/users/details/" + url;
        },
        position: "last"
    });

    jQuery("#jqGrid").navButtonAdd("#jqGridPager", {
        caption: "حذف",
        title: "حذف",
        buttonicon: "ui-icon ui-icon-trash",
        onClickButton: function () {

            var selectedRowId = jQuery("#jqGrid").jqGrid("getGridParam", "selrow");
            if (!selectedRowId)
                return;

            var selectedRow = jQuery("#jqGrid").jqGrid("getRowData", selectedRowId);
            var url = selectedRow['id'];
            if (url)
                window.location.href = "/users/delete/" + url;
        },
        position: "last"
    });


    resizeJqGridWidth('jqGrid', 'divMain', '80%');
};


var UserDeletePage = function() {
    
    $.ajax({
        url: "/Users/DetailsJson/" + $("#urlId").val(),
        type: 'Get',
    dataType: 'json',
    contentType: 'application/json',
    success: function (data)
    {
        $("#id").text(data.id);
        $("#username").text(data.userName);
        $("#firstname").text(data.firstName);
        $("#lastname").text(data.lastName);

        if (0 === data.roles.length)
        {
            $(".userRoles").html("<p> <i> No Role Assigned</i> </p>");
            $("#msgConfirm").removeClass("hidden");
            $("#deleteBtn").on("click", function() {
                $.ajax({
                    url: "/Users/Delete/" + data.id,
                    type: 'Post',
                    success: function (data)
                    {
                        window.location = "/users/index";
                    },
                    error: function (request, status, error)
                    {
                        alert(request.responseText);
                    }
                });
            });
        }
        else
        {
            $("#msgCantDelete").removeClass("hidden");
            $("#editBtn").attr("href", "/Users/Details/" + data.id);
            $.each(data.roles, function (index, elem)
            {
                $(".userRoles").append(
                    "<div class='hoverable'>" + elem.applicationName + " - " + elem.name + (elem.description == null ? "" : " - " + elem.description) +
                    "<a class='btn btn-xs btn-primary pull-right' href='/Roles/Details/" + elem.id + "'> details</a></div>");
            });
        }
    },
    error: function (request, status, error)
    {
        alert(request.responseText);
    }
});
};


var UserDetailsPage = function() {
    $.ajax({
        url: "/Users/DetailsJson/" + $("#urlId").val(),
        type: 'Get',
        dataType: 'json',
        contentType: 'application/json',
        success: function(data) {
            $("#id").text(data.id);
            $("#username").text(data.userName);
            $("#firstname").text(data.firstName);
            $("#lastname").text(data.lastName);

            $(".addRoleForm").attr("action", "/Users/Assign/" + data.id);

            if (0 === data.roles.length) {
                $(".userRoles").html("<p> <i> No Role Assigned</i> </p>");
            } else {
                $.each(data.roles,
                    function(index, elem) {
                        $(".userRoles").append(
                            "<div class='hoverable'>" +
                            elem.applicationName +
                            " - " +
                            elem.name +
                            (elem.description == null ? "" : " - " + elem.description) +
                            "<a class='btn btn-xs btn-danger pull-right deleteButton'  href='/Users/UnAssign/" +
                            data.id +
                            "?roleId=" +
                            elem.id +
                            "'> delete </a>" +
                            "<a class='btn btn-xs btn-primary pull-right' href='/Roles/Details/" +
                            elem.id +
                            "'> details</a></div>");
                    });
            }

            $.each(data.otherRoles,
                function(index, elem) {
                    $(".selectOtherRoles").append(
                        "<option value='" +
                        elem.id +
                        "'> " +
                        elem.applicationName +
                        " - " +
                        elem.name +
                        (elem.description == null ? "" : " - " + elem.description) +
                        "</option>");
                });


            $(".deleteButton").on("click",
                function(e) {
                    var link = this;
                    e.preventDefault();

                    $("<div>Are you sure you want to delete?</div>").dialog({
                        buttons: {
                            "Ok": function() { window.location = link.href; },
                            "Cancel": function() { $(this).dialog("close"); }
                        }
                    });
                });
        },
        error: function(request, status, error) {
            alert(request.responseText);
        }
    });
};