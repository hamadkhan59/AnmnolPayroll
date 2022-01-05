function GetDatePickerOptions() {
    var optionSet1 = {
        startDate: moment().subtract(29, 'days'),
        endDate: moment(),
        minDate: moment().subtract(10, 'years'),
        maxDate: moment(),
        //dateLimit: {
        //    days: 365
        //},
        //maxSpan: {
        //    days: 365
        //},
        //dateLimit: {
        //    days: 365
        //},
        showDropdowns: true,
        "linkedCalendars": false,
        showWeekNumbers: false,
        timePicker: false,
        timePickerIncrement: 1,
        timePicker12Hour: true,
        ranges: {
            'Today': [moment(), moment()],
            'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            'Last 7 Days': [moment().subtract(6, 'days'), moment()],
            'Last 30 Days': [moment().subtract(29, 'days'), moment()],
            'This Month': [moment().startOf('month'), moment()],
            'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        },
        opens: 'left',
        buttonClasses: ['btn btn-default'],
        applyClass: 'btn-small btn-primary',
        cancelClass: 'btn-small',
        format: 'DD/MM/YYYY',
        separator: ' to ',
        locale: {
            applyLabel: 'Search',
            cancelLabel: 'Clear',
            fromLabel: 'From',
            toLabel: 'To',
            customRangeLabel: 'Custom',
            daysOfWeek: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
            monthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
            firstDay: 1
        }
    };

    return optionSet1;
}

function InitializeDatePicker(containerId, onApplyCallBack, opens) {
    if (opens == null) {
        opens = 'left';
    }

    var options = GetDatePickerOptions();
    options.opens = opens;

    var cb = function (start, end, label) {
        $('#' + containerId + ' span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
    };

    $('#' + containerId + ' span').html(moment().subtract(29, 'days').format('MMMM D, YYYY') + ' - ' + moment().format('MMMM D, YYYY'));
    $('#' + containerId).daterangepicker(options, cb);
    $('#'+ containerId).on('apply.daterangepicker', function (ev, picker) {
        onApplyCallBack(picker.startDate._d, picker.endDate._d);
    })//end json
    $('#' + containerId).on('cancel.daterangepicker', function (ev, picker) {});

    $('#destroy').click(function () {
        $('#feeStatsDatePicker').data('daterangepicker').remove();
    });
}
