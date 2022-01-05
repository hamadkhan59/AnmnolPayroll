function GetClassSection(classId, loadAll, elemntId, payload, sectionId) {
    //first : classId, seccond : load all option or not, third : section dd html id 
    //fourth : any call function to call, fifth : sectionId to set in DD

    $.ajax({
        type: "GET",
        url: AppDetail() + '/api/Common/getSections?classId=' + classId + '&&isLoadAll=' + loadAll,
        contentType: 'application/json',
        success: function (result) {
            $("#" + elemntId).html(result);
            if (sectionId > 0) {
                $("#" + elemntId).val(sectionId);
            }
            if (payload == 1) {
                getRollNumber();
            }
            else if (payload == 2) {
                setPrevSearchVals();
            }
            else if (payload == 3) {
                setPrevSearchVals();
                getSubjectList();
            }

        },
        error: function (res) {
            showNotification('Unable to load Sections', 'Error', 'Error')
        }
    });
}

function ResetBioMatricCount() {

    $.ajax({
        type: "GET",
        url: AppDetail() + '/api/Common/UpdateBioMatrixLogCount',
        contentType: 'application/json',
        success: function (result) {
            showNotification('Machine Logs are reset on the server successfully', 'Success', 'Success');

            var myInterval = setTimeout(function () {
                location.reload();
            }, 2000);
        },
        error: function (res) {
            showNotification('Unable to Reset Machine Logs on the server', 'Error', 'Error');
            var myInterval = setTimeout(function () {
                location.reload();
            }, 2000);

        }
    });
}

function GetDesignation(categoryId, loadAll, elemntId) {

    $.ajax({
        type: "GET",
        url: AppDetail() + '/api/Common/getDesignation?categoryId=' + categoryId + '&&isLoadAll=' + loadAll,
        contentType: 'application/json',
        success: function (result) {
            $("#" + elemntId).html(result);
        },
        error: function (res) {
            showNotification('Unable to load designations', 'Error', 'Error')
        }
    });
}




function getRollNumber() {

    var classId = $("#classId").val();
    var classIdTemp = $("#txtClassIdHidden").val();
    var sectionId = $("#sectionId").val();
    var sectionIdTemp = $("#txtSectionIdHidden").val();
    var rollNo = $("#txtRollNoHidden").val();

    if ((classIdTemp == 0 && sectionIdTemp == 0) || classId != classIdTemp || sectionId != sectionIdTemp) {
        $.ajax({
            url: AppDetail() + '/Student/GetRollNumber',
            type: 'Get',
            contentType: 'application/json',
            dataType: 'json',
            data: { 'classId': classId, 'sectionId': sectionId },
            statusCode: {
                200: function (data) {
                    //alert(JSON.stringify(data))
                    //var json2 = JSON.parse(data);
                    $('#rollNumber').val(data);
                },
                500: function () {
                }
            },
        });
    }
    else {
        $('#rollNumber').val(rollNo);
    }
}



function getStaffPaymentApproval(staffId, message, buttonId) {

    $.ajax({
        url: AppDetail() + '/StaffSalary/GetStaffPaymentApproval',
        type: 'Get',
        contentType: 'application/json',
        dataType: 'json',
        data: { 'staffId': staffId },
        statusCode: {
            200: function (data) {
                if (data != 0) {
                    ConfirmAction(message, buttonId);
                }
                else {
                    showNotification('Staff Payment Approval is not found, Please take approval from staff to process payment', 'Error', 'Error');
                }
            },
            500: function () {
            }
        },
    });

    return false;
}

function GetDriverStop() {
    var driverId = $('#driverStopId').val();
    $.ajax({
        type: "GET",
        url: AppDetail() + '/api/Common/getDriverStops?driverId=' + driverId,
        contentType: 'application/json',
        success: function (result) {
            $('#stopBody > tbody').html("");
            $("#stopBody").find('tbody').append(result);
            GetVanStrength();
        },
        error: function (res) {
            showNotification('Unable to load Sections', 'Error', 'Error')
        }
    });
}

function GetVanStrength() {
    var driverId = $('#driverStopId').val();
    $.ajax({
        url: AppDetail() + '/Student/GetVanStrength',
        type: 'Get',
        contentType: 'application/json',
        dataType: 'json',
        data: { 'driverId': driverId },
        statusCode: {
            200: function (data) {
                //alert(JSON.stringify(data))
                //var json2 = JSON.parse(data);
                $('#vanStrength').val(data);
            },
            500: function () {
            }
        },
    });
}

function GetTermsByYear(yearId, loadAll, elemntId, branchId, payload, termId) {

    $.ajax({
        type: "GET",
        url: AppDetail() + '/api/Common/getTermByYear?yearId=' + yearId + '&&branchId=' + branchId + '&&isLoadAll=' + loadAll,
        contentType: 'application/json',
        success: function (result) {
            $("#" + elemntId).html(result);
            if (termId > 0) {
                $("#" + elemntId).val(termId);
            }
            if (payload == 1) {
                GetExamByTerm();
            }
        },
        error: function (res) {
            showNotification('Unable to load terms', 'Error', 'Error')
        }
    });
}

function GetExamsByTerm(termId, loadAll, elemntId, branchId, payload, examId) {

    $.ajax({
        type: "GET",
        url: AppDetail() + '/api/Common/getExamByTerm?termId=' + termId + '&&branchId=' + branchId + '&&isLoadAll=' + loadAll,
        contentType: 'application/json',
        success: function (result) {
            $("#" + elemntId).html(result);
            if (yearId > 0) {
                $("#" + elemntId).val(examId);
            }
        },
        error: function (res) {
            showNotification('Unable to load exams', 'Error', 'Error')
        }
    });
}

