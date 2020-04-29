
$(document).ready(function () {
   FilteredStatusGrid()
});


function FilteredStatusGrid() {
    
    debugger
    var pathiframe = $("#Iframepath").val();

    $("#Reloader").html('<iframe id="Reloader" height="300px" width="100%" scrolling="yes" frameborder="0" src=' + pathiframe + '></iframe>');
    var urlBindRecordsToGrid = $("#urlBindRecordsToGrid").val();  // url for active records
    //$.ajax({
    //    url: urlBindRecordsToGrid,
    //   // data: JSON.stringify(StatusUpdateVM),
    //    contentType: "application/json; charset=utf-8",
    //    dataType: "json",
    //    async: true,

    //    type: 'POST',
    //    success: function (data) {
    //        $("#Reloader").html('<iframe id="Reloader" height="300px" width="100%" scrolling="yes" frameborder="0" src=' + pathiframe + '></iframe>');


    //    }
    //});

}