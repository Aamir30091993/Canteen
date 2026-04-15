//Reporting
$('#ddlReporting').change(function () {
    debugger;
    var report = $('#ddlReporting').val();
    $('#UserDetails_ReportingToUserID').val(report);
    debugger;

});


//Role
$('#ddlRole').change(function () {
    debugger;
    var role = $('#ddlRole').val();
    $('#UserDetails_RoleID').val(role);
    debugger;

});

//Location
$('#ddlLocation').change(function () {
    debugger;
    var loc = $('#ddlLocation').val();
    $('#UserDetails_LocationID').val(loc);
    debugger;

});

//Date 1
//$(document).ready(function () {
//    debugger;
//    $("#UserDetails_DOJ").datepicker({
//        autoclose: true,
//        todayHighlight: true,
//        format: 'dd/mm/yyyy'   /*dd M yyyy*/
//    });
//});


//$(document).ready(function () {
//    debugger;
//    $("#UserDetails_DOL").datepicker({
//        autoclose: true,
//        todayHighlight: true,
//        format: 'dd/mm/yyyy'   /*dd M yyyy*/
//    });
//});

//Date 2
//$(function () {
//    debugger;
//    var MinimimDate = new Date();
//    MinimimDate.setDate(MinimimDate.getDate());

//    $("#UserDetails_DOJ").datepicker({
//        startDate: MinimimDate,
//        // endDate: new Date(),

//        autoclose: true,
//        todayHighlight: true
//    });
//});


//$(function () {
//    debugger;
//    $("#UserDetails_DOJ").datepicker(
//        {

//            todayHighlight: 'TRUE',
//            format: 'dd/mm/yyyy',
//            autoclose: true,
//            startDate: new Date()

//        }).on('changeDate', function (ev) {
//            debugger;
//            ev.date.toDateString()

//        });
//    debugger;
//});


//$(function () {
//    debugger;
//    var MinimimDate = new Date();
//    MinimimDate.setDate(MinimimDate.getDate());

//    $("#UserDetails_DOL").datepicker({
//        startDate: MinimimDate,
//        // endDate: new Date(),

//        autoclose: true,
//        todayHighlight: true
//    });
//});


//$(function () {
//    debugger;
//    $("#UserDetails_DOL").datepicker(
//        {

//            todayHighlight: 'TRUE',
//            format: 'dd/mm/yyyy',
//            autoclose: true,
//            startDate: new Date()

//        }).on('changeDate', function (ev) {
//            debugger;
//            ev.date.toDateString()

//        });
//    debugger;
//});


//Date3

$(function () {
    debugger;
    //var MinimimDate = new Date();
    //MinimimDate.setDate(MinimimDate.getDate());
    $("#UserDetails_DOJ").datepicker(
        {
            todayHighlight: 'TRUE',
            format: 'dd/mm/yyyy',
            autoclose: true,
            startDate: '01/01/1900'//MinimimDate//new Date()

        }).on('changeDate', function (ev) {
            debugger;
            ev.date.toDateString()


        });
});



$(function () {
    debugger;
    //var MinimimDate = new Date();
    //MinimimDate.setDate(MinimimDate.getDate());
    $("#UserDetails_DOL").datepicker(
        {
            todayHighlight: 'TRUE',
            format: 'dd/mm/yyyy',
            autoclose: true,
            startDate: '01/01/1900'//MinimimDate//new Date()

        }).on('changeDate', function (ev) {
            debugger;
            ev.date.toDateString()
        });
});
