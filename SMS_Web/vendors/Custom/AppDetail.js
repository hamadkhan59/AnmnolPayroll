function AppDetail()
{
    var appName = '';
    return location.protocol + "//" + location.host + appName;
}


function ShowSearchPanel(Count) {
    if (Count > 0)
    {
        //document.getElementById("searchPanelData").style.display = "block";
        $('#searchPanelData').fadeIn('200000');
        //$("#searchPanelData").slideDown("slow");
    }
    //else
    //{
    //    document.getElementById("searchPanelData").style.display = "none";
    //}
}

function DoDelete() {
    DoHide();
    //$("#deleteStatus").val(1);
    var controllerName = $("#ControllerName").val();
    var buttonId = $("#buttonId").val();
    if (buttonId != null && buttonId.length > 0) {
        document.getElementById(buttonId).onclick = true;
        $("#" + buttonId).click();

        document.getElementById(buttonId).disabled = true;

        if(buttonId == 'fastPaid')
        {
            SaveChallanDetail();
        }
        else if(buttonId == 'saveSalaryBtn')
        {
            UploadStaffData();
        }
        else if(buttonId == 'resetLogs')
        {
            ResetBioMatricCount();
        }
        else if (buttonId == 'btnJournalEntry' || buttonId == 'btnBPEntry' 
            || buttonId == 'btnCPEntry' || buttonId == 'btnBREntry' || buttonId == 'btnCREntry')
        {
            PostJournalVoucher();
        }
    }
    else{
        var deleteIndex = $("#deleteIndex").val();
        window.location.href = AppDetail() + '/' + controllerName + '/Delete/' + deleteIndex;
    }
}

function DoHide()
{
    //$("#deleteStatus").val(0);
    var buttonId = $("#buttonId").val();
    if(document.getElementById(buttonId) != null)
    {
        document.getElementById(buttonId).disabled = false;
    }
    $("#mi-modal").modal('hide');
}

function ConfirmDelete(deleteText, deleteId, controllerName)
{
    $("#deleteText").text(deleteText);
    $("#deleteIndex").val(deleteId);
    $("#ControllerName").val(controllerName);

    $("#mi-modal").modal('show');
    
    return false;
}


function ConfirmAction(actionText, buttonId) {
    $("#deleteText").text(actionText);
    $("#buttonId").val(buttonId);

    $("#mi-modal").modal('show');

    return false;
}

function ConfirmActionReset(actionText, buttonId) {
    $("#deleteText").text(actionText);
    $("#buttonId").val(buttonId);

    $("#mi-modal").modal('show');

}

function showNotification(message, title, type) {
    toastr.options = { 
        "positionClass": "toast-container",
    };

    switch (type) {
        case "Info":
            toastr.info(message, title);
            break;
        case "Success":
            toastr.success(message, title);
            break;
        case "Warning":
            toastr.warning(message, title);
            break;
        case "Error":
            toastr.error(message, title);
            break;
    }
}

function SetDayViewOneMonth(fromDate, toDate, view)
{
    const date1 = moment(fromDate).format("MM/DD/YYYY");
    const date2 = moment(toDate).format("MM/DD/YYYY");
    var stMonth = date1.split("/")[0];
    var stDay = date1.split("/")[1];
    var endmonth = date2.split("/")[0];
    var endDay = date2.split("/")[1];

    var days = (endmonth - stMonth) * 30;
    var remDays = endDay - stDay;
    days += remDays;
    if (view == "day" && days > 31)
    {
        showNotification('For Day View Please Select the date range within 30 Days.', 'Error', 'Error')
        return false;
    }

    return true;
}

function populateValidationErrors() {
    var summaryUl = $(".validation-summary-errors").find("ul");
    var items = summaryUl.find("li");
    var jsData = '';
    for (i = 0; i < items.length; i++) {
        //if (i == 0) {
        //    jsData += items[i].innerHTML;
        //}
        //else {
        //    jsData += ", " + items[i].innerHTML;
        //}
        showNotification(items[i].innerHTML, 'Error', 'Error')
    }
    //showNotification(jsData, 'Error', 'Error')
}

