
$(document).on("click", ".XLS", function () {
    debugger;
    //window.location.href = "/Reports/Export?ReportName=" + $("#ReportName").val().replace(/\&/g, '') + "&ReportID=" + $("#ReportParameterDetails_ReportTypeID").val() + "&SessionIdentifier=" + $("#hdfSessionIdentifier").val();
    window.location.href = "/Report/Export?ReportName=" + $("#ReportName").val() + "&ReportID=" + $("#ReportParameterDetails_ReportID").val() + "&SessionIdentifier=" + $("#hdfSessionIdentifier").val();
});
