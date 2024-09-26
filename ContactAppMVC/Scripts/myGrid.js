$(document).ready(function () {
    $("#grid").jqGrid({
        url: "/ContactDetail/GetData",
        datatype: "json",
        colNames: ['Id', 'Type', 'Value'],
        colModel: [{ name: "Id", key: true, hidden: true },
        { name: "Type", editable: true, searchoptions: { sopt: ['eq'] } },
        { name: "Value", editable: true, search: false }],
        height: "250",
        caption: "Contact Detail Records",
        pager: "#pager",
        rowNum: 5,
        rowList: [5, 10, 15],
        sortname: 'Id',
        sortorder: 'asc',
        viewrecords: true,
        width: "650",
        gridComplete: function () {
            $("#grid").jqGrid('navGrid', "#pager", { edit: true, add: true, del: true, refresh: true },
                {
                    url: "/ContactDetail/Edit",
                    closeAfterEdit: true,
                    width: 600,
                    afterSubmit: function (response, postdata) {
                        var result = JSON.parse(response.responseText);
                        if (result.success) {
                            alert(result.message)
                            return [true]

                        } else {
                            alert(result.message)
                            return [false]
                        }
                    }
                }, {
                url: "/ContactDetail/Add",
                closeAfterAdd: true,
                width: 600,

                afterSubmit: function (response, postdata) {
                    var result = JSON.parse(response.responseText);
                    if (result.success) {
                        return [true]
                    }
                    else {
                        return [false]
                    }

                }
            }, {
                url: "/ContactDetail/Delete",
                afterSubmit: function (response, postdata) {
                    var result = JSON.parse(response.responseText);
                    if (result.success) {
                        alert(result.message)
                        return [true];
                    } else {
                        alert(reslt.message);
                        return [false];
                    }
                }
            },
                {
                    multipleSearch: false,
                    closeAfterSearch: true
                }),
                $("#refreshbutton").click(function () {
                    $("#grid").jqGrid('setGridparam', { search: false });
                    $("#grid").jqGrid('setGridParam', { page: 1 }).trigger('reloadGrid')
                })
        }
    });

    console.log("jqGrid initialized.");
});
