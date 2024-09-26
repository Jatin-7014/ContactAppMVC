function loadItems() {
    $.ajax({
        url: "/Contact/GetContactById",
        type: "GET",
        dataType: "json",
        success: function (data) {
            $("#tblBody").empty();
            $.each(data, function (index, item) {
                var row = `<tr>
                <td>${item.FName}</td>
                <td>${item.LName}</td>
                <td>
                <input type="checkbox" class="checkbox" data-user-id="${item.Id}" ${item.IsActive ? "checked" : ""}/>
                </td>
                <td>
                <button onclick="EditContact(${item.Id})" class="btn-btn-success">Edit</button>
                <form action="/ContactDetail/Index" method="POST" style="display:inline;">
                <input type="hidden" name="id" value="${item.Id}" />
                <button type="submit" class="btn btn-warning">Get Contact Details</button>
                </form>
                </td>
                </td>`
                $("#tblBody").append(row)
            })
        },
        error: function (err) {
            console.log(err);
            $("#tblBody").empty()
            alert("No data available")
        }
    })
}
function EditContact(itemId) {
    $.ajax({
        url: "/Contact/GetContact",
        type: "GET",
        data: { id: itemId },
        success: function (success) {
            $("#contactId").val(success.Id),
                $("#fname").val(success.FName),
                $("#lname").val(success.LName)
            $(".listItems").hide()
            $("#editRecord").show()

        },
        error: function (err) {
            console.log(itemId)
            alert("Error:Operation On De-Activated Account")
        }
    })
}
function addNewRecord(item) {
    $.ajax({
        url: "/Contact/Create",
        type: "POST",
        data: item,
        success: function (postItem) {
            alert("Contact Added Successfully.")
            loadItems()
        },
        error: function (err) {
            alert("Error occur during the creation of user.")
        }
    })

}
$(document).ready(function () {
    $("#tblBody").on('change', '.checkbox', function () {
        console.log("Change Occured")
        var checkbox = $(this);
        var userId = checkbox.data('user-id');
        var isActive = checkbox.is(':checked');
        if (confirm("Do you really wish to continue ?")) {
            $.ajax({
                url: '/Contact/EditContactStatus',
                type: 'POST',
                data: {
                    userId: userId,
                    isActive: isActive
                },
                success: function (success) {
                    alert("Contact Status Updated Successfully")
                    //loadItems()
                },
                error: function (err) {
                    alert("Error in deactivating contact")
                    checkbox.prop('checked', !isActive)
                }


            })
        } else {

            checkbox.prop('checked', !isActive);
        }
    })

})
//toggling buttons
$("#btnAdd").click(() => {
    $(".listItems").hide()
    $("#newRecord").show()
})