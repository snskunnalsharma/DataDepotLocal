
$(document).ready(function () {

    //$("#ddlDashboardStatus").chosen({
    //    disable_search_threshold: 10,
    //    no_results_text: "Oops, nothing found!",
       
    //});


    //$("#ddlDashboardProjects").chosen({
    //    // disable_search_threshold: 10,
    //    no_results_text: "Oops, nothing found!",
       
    //});

    //$("#ddlDashboardCurrentCity").chosen({
    //    //disable_search_threshold: 10,
    //    no_results_text: "Oops, nothing found!",
       
    //});

    bindCityPageHeader()

});


$('#ddlDashboardStatus').change(function () {
    var StatusID = $('#ddlDashboardStatus').val();

    $('#ddlDashboardCurrentCity').empty();
    $('#ddlDashboardCurrentCity').append($('<option></option>').val('').html("--Select City--"));
   // $("#ddlDashboardCurrentCity").trigger("chosen:updated");
    if (StatusID != "") {

        bindCityPageHeader(StatusID)
    }
})

function bindCityPageHeader(StatusID) {
    
    var urlGetCityListHeader = $('#urlGetCityListHeader').val();

    $.ajax({
        url: urlGetCityListHeader,
        type: 'GET',
        data:{StatusID:StatusID},
        success: function (cityList) {
            $('#ddlDashboardCurrentCity').empty();
            $('#ddlDashboardCurrentCity').append($('<option></option>').val('').html("--Select City--"));

            $.each(cityList, function (index, item) {

                $('#ddlDashboardCurrentCity').append(
                       $('<option></option>').val(item.CityID).html(item.CityName)
                 );
            });

            $("#ddlDashboardCurrentCity").trigger("chosen:updated");

        },
        error: function (jqXHR, textStatus, errorThrown) {
            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
            console.log('jqXHR:');
            console.log(jqXHR);
            console.log('textStatus:');
            console.log(textStatus);
            console.log('errorThrown:');
            console.log(errorThrown);
        },
    });
}

$('#ddlDashboardCurrentCity').change(function () {

    var cityID = $('#ddlDashboardCurrentCity').val();
   
    $('#ddlDashboardProjects').empty();
    $('#ddlDashboardProjects').append($('<option></option>').val('').html("--Select Project--"));
   // $("#ddlDashboardProjects").trigger("chosen:updated");
    if (cityID != "") {

        bindProjectbyCityPageHeader(cityID)
    }
})

function bindProjectbyCityPageHeader(cityID) {

    if (cityID != "" && cityID != undefined) {
        var urlGetProjectListbyCity = $('#urlGetProjectListbyCity').val();

        $.ajax({
            url: urlGetProjectListbyCity,
            type: 'GET',
            data: { CityID: cityID },
            success: function (projectList) {
                
                // $("#ddlDashboardProjects").chosen("destroy");
                $('#ddlDashboardProjects').empty();
                $('#ddlDashboardProjects').append($('<option></option>').val('').html("--Select Project--"));

                $.each(projectList, function (index, item) {

                    $('#ddlDashboardProjects').append(
                           $('<option></option>').val(item.ProjectID).html(item.ProjectName)
                     );
                });
                $("#ddlDashboardProjects").trigger("chosen:updated");


            },
            error: function (jqXHR, textStatus, errorThrown) {
                var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
                console.log('jqXHR:');
                console.log(jqXHR);
                console.log('textStatus:');
                console.log(textStatus);
                console.log('errorThrown:');
                console.log(errorThrown);
            },
        });
    }
}

$('#ddlDashboardProjects').change(function () {
   
})