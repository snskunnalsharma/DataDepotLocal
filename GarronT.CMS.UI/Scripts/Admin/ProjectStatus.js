var spinner, opts, target;
var oTable;
var StateData;
var BindCon;
$(document).ready(function () {
    debugger;
    $("#ProjectStatus").addClass('active').css('background-color', '#EF6C00').css('border-radius', '6px');
    $("#ProjectStatus").children().css('color', '#fff');
    $("#ddlProjectStatus").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlProject").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlInstaller").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlLocation").chosen({ no_results_text: "Oops, nothing found!" });
    //AddedBy Aniket on 15-Oct-2016
    $("#ddlUtilityType").chosen({ no_results_text: "Oops, nothing found!" });
    $("#ddlformtype").chosen({ no_results_text: "Oops, nothing found!" });
    debugger;
    AddRowPartial()

    resizeContent()

});


function BindFormType() {
    debugger;
    var Project = $("#ddlProject option:selected")
    var fromtype = $("#ddlformtype option:selected")

    if (Project.length > 0) {
        var GetFormType = $('#urlGetFormType').val();
        $.ajax({
            type: 'GET',
            url: GetFormType,
            success: function (data) {
                $('#ddlformtype').empty();
                var count = data.length;
                if (count > 0) {
                    $.each(data, function (index, item) {
                        if (index == 0) {
                            $('#ddlformtype').append($('<option></option>').val('0'));
                        }
                        $('#ddlformtype').append($('<option></option>').val(item.ID).html(item.FormType));
                    });
                    $('.chosen-select').trigger('chosen:updated');
                }

            },
            error: function () {
                var $toast = toastr["error"]("something seems wrong", "Error Notification");
            }
        });
    }
}

function LocationList() {
    debugger;
    var Project = $("#ddlProject option:selected")

    if (Project.length > 0) {
        var Getlocation = $('#urlGetLocationList').val();
        $.ajax({
            type: 'GET',
            url: Getlocation,
            data: { projectId: Project.val()},
            success: function (data) {
                $('#ddlLocation').empty();
                var count = data.length;
                if (count > 0) {
                    debugger;
                    $.each(data, function (index, item) {
                        if (index == 0) {
                            $('#ddlLocation').append($('<option></option>').val('0'));
                        }
                        $('#ddlLocation').append($('<option></option>').val(item.LocationId).html(item.LocationName));
                    });
                    $('.chosen-select').trigger('chosen:updated');
                }

            },
            error: function () {
                var $toast = toastr["error"]("something seems wrong", "Error Notification");
            }
        });
    }
}

$('#ddlUtilityType').change(function () {
    BindProjects()

})

function BindProjects() {

    var projectUtilityType = $('#ddlUtilityType').val() == "0" ? "no" :
       ($('#ddlUtilityType').val() == 1 ? "Electric" : ($('#ddlUtilityType').val() == 2 ? "Gas" : "Water"));


    if ($('#ddlProjectStatus').val() == "0") {
        resetData();
    }
    else {
        $('#ddlProject').empty();
        $('#ddlProject').trigger("chosen:updated");
        var GetProject = $('#urlGetProject').val();
        var _proStatus = $('#ddlProjectStatus').val();
        $.ajax({
            type: 'GET',
            url: GetProject,
            async: false,
            data: { "ProjectStatus": _proStatus, "utilityType": projectUtilityType },
            success: function (data) {
                $('#ddlProject').empty();

                var count = data.length;
                if (count > 0) {

                    $.each(data, function (index, item) {
                        if (index == 0) {
                            $('#ddlProject').append(
                              $('<option></option>').val('0'));
                        }
                        $('#ddlProject').append(
                                              $('<option></option>').val(item.ProjectId).html(item.ProjectCityState)
                                        );
                    });
                    $('.chosen-select').trigger('chosen:updated');
                }

            },
            error: function () {
                var $toast = toastr["error"]("something seems wrong", "Error Notification");
            }

        });
    }

}


//get the project list as per the project status selected
$('#ddlProjectStatus').change(function () {

    BindProjects()


});




function resetData() {
    debugger;


    $('#ddlProject').empty();
    $('#ddlProject').trigger("chosen:updated");

    $('#ddlInstaller').empty();
    $('#ddlInstaller').trigger("chosen:updated");

    $('#ddlLocation').empty();
    $('#ddlLocation').trigger("chosen:updated");
}

function resizeContent() {

    var $height = $(document).height() - 242;

    $('#LoadRow').height($height);
    // $('#LoadRow').width($width);
}


$('#ddlProject').change(function () {

    var Project = $("#ddlProject option:selected")
    $('#ddlInstaller').empty();
    $('#ddlInstaller').trigger("chosen:updated");
    $('#ddlformtype').empty();
    $('#ddlformtype').trigger("chosen:updated");

    if (Project.length > 0) {
        BindFormType();
        LocationList();
    }


})

$('#ddlformtype').change(function () {


    $('#ddlInstaller').empty();
    $('#ddlInstaller').trigger("chosen:updated");


    var Project = $("#ddlProject option:selected")
    var formType = $("#ddlformtype option:selected")
    // GetAddress(Project.val(), 0)

    if (formType.length > 0) {

        var urlGetInstallerList = $('#urlGetInstallerList').val();

        $.ajax({
            type: 'GET',
            url: urlGetInstallerList,
            data: { projectId: Project.val(), formType: formType.val() },
            beforeSend: function () {
                waitingDialog.show('Loading Please Wait...');
            },
            complete: function () {
                //waitingDialog.hide();
            },
            success: function (data) {
                debugger;
                $('#ddlInstaller').empty();

                var count = data.length;
                if (count > 0) {



                    $('#ddlInstaller').append(
                             $('<option></option>').val('0').html("All"));

                    $.each(data, function (index, item) {
                        if (index == 0) {

                        }
                        $('#ddlInstaller').append(
                                              $('<option></option>').val(item.UserId).html(item.UserName)
                                        );
                    });
                    $('.chosen-select').trigger('chosen:updated');
                }
                waitingDialog.hide();
                if (formType == 2) {
                    $('#ddlInstaller').attr('data-placeholder', 'Select Auditor');
                }
                else if (formType == 1) {
                    $('#ddlInstaller').attr('data-placeholder', 'Select Installer');
                }
                else {
                    $('#ddlInstaller').attr('data-placeholder', 'Select');
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                waitingDialog.hide();
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
});




$('#btnShowMap').click(function () {



    var ProjectID = $("#ddlProject option:selected").val();

    var FormTypeID = $("#ddlformtype option:selected").val();
    debugger;
    if ($("#ddlUtilityType option:selected").val() == "0" || $("#ddlUtilityType option:selected").val() === undefined) {
        var $toast = toastr["error"]("Please, select the Utility Type first.", "");
    }
    else if ($("#ddlProjectStatus option:selected").val() === "0" || $("#ddlProjectStatus option:selected").val() === undefined) {
        var $toast = toastr["error"]("Please, select the Project Status first.", "");
    }
    else if (ProjectID === "0" || ProjectID === undefined) {
        var $toast = toastr["error"]("Please, select the project first.", "");
    }
    else if (FormTypeID == "0" || FormTypeID === undefined) {
        var $toast = toastr["error"]("Please, select the form type first.", "");
    }
    else {

        var InstallerID = $("#ddlInstaller option:selected").val();
        var LocationId = $("#ddlLocation option:selected").val();
        if (LocationId === undefined) {
            LocationId = "0";
        }
        debugger;
        if (InstallerID === undefined) {
            var $toast = toastr["error"]("Sorry you can not proceed as Audit process not configured for selected project.", "");
        }
        else if (InstallerID === '0') {
            GetAddress(ProjectID, 0, FormTypeID, LocationId)
        }
        else {
            GetAddress(ProjectID, InstallerID, FormTypeID, LocationId)
        }
    }

});

function AddRowPartial() {


    var urlgetRowPartial = $('#getRowPartial').val();
    $.get(urlgetRowPartial,
     function (data) {
         $("#LoadRow").append(data)
         initialize();
     });


}

function initialize() {

    //var lat = document.getElementById('txtlat').value;
    //var lon = document.getElementById('txtlon').value;
    //var myLatlng = new google.maps.LatLng(18.5004866, 73.8668996) // This is used to center the map to show our markers
    var myLatlng = new google.maps.LatLng(39.86911500000000, -100.891880000000) // This is used to center the map to show our markers
    var mapOptions = {
        center: myLatlng,
        zoom: 05,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        marker: false
    };

    var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
    map.setOptions({ minZoom: 3 });
    var marker = new google.maps.Marker({
        position: myLatlng
    });
    //  marker.setMap(map);


}

$('#map_canvas').addClass("canvas-map");

function GetAddress(projectID, InstallerID, FormTypeID, LocationId) {
    debugger;
    var dd = LocationId;
    debugger;
    // var projectID = 1;
    var urlGetAddressMap = $('#urlGetAddressMap').val();

    $.ajax({
        type: 'GET',
        url: urlGetAddressMap,
        async: true,
        data: { projectID: projectID, InstallerID: InstallerID, formTypeId: FormTypeID, locationId: LocationId},
        // contentType: "application/json; charset=utf-8",
        // dataType: "json",
        beforeSend: function () {
            waitingDialog.show('Loading Please Wait...');
        },
        complete: function () {
            waitingDialog.hide();
        },
        success: function (data) {
            debugger;

            ShowMap(data)

        },
        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            var $toast = toastr["error"](textStatus + "</br>" + errorThrown, "Error Notification");
            console.log('jqXHR:');
            console.log(jqXHR);
            console.log('textStatus:');
            console.log(textStatus);
            console.log('errorThrown:');
            console.log(errorThrown);
        }

    });


}

function ShowMap(objectMap) {
    debugger;
    // waitingDialog.show('Loading Please Wait...');
    if (objectMap.length > 0) {


        latlongList = objectMap;

        var latlngbounds = new google.maps.LatLngBounds();
        var infoWindow = new google.maps.InfoWindow();

        var mapOptions = {
            center: new google.maps.LatLng(objectMap[0].latitude, objectMap[0].longitude),
            zoom: 06,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
        //var installername = $('#ddlInstaller option:selected').text();
        //var installerid = $('#ddlInstaller option:selected').val();

        var markers = new Array();
        // markers = initMarkers(objectMap);



        //4-completed
        //3-skipped

        //2-pending
        //1-not mapped
        //5-partially done
        for (i = 0; i < objectMap.length; i++) {

            var data = objectMap[i];

            var myLatlng = new google.maps.LatLng(data.latitude, data.longitude);
            var marker;
            if (objectMap[i].colorFlag == 1) {
                marker = new google.maps.Marker({
                    position: myLatlng,
                    map: map,
                    animation: google.maps.Animation.DROP,
                    label: {
                        fontFamily: 'Roboto, Arial, sans-serif'
                    },
                    icon: 'https://maps.google.com/mapfiles/ms/icons/blue-dot.png'

                }
                );
            }
            else if (objectMap[i].colorFlag == 2) {
                marker = new google.maps.Marker({
                    position: myLatlng,
                    map: map,
                    animation: google.maps.Animation.DROP,

                    label: {
                        fontFamily: 'Roboto, Arial, sans-serif'
                    },
                    icon: 'https://maps.google.com/mapfiles/ms/icons/orange-dot.png'


                }
                );
            }
            else if (objectMap[i].colorFlag == 3) {
                marker = new google.maps.Marker({
                    position: myLatlng,
                    map: map,
                    animation: google.maps.Animation.DROP,

                    label: {
                        fontFamily: 'Roboto, Arial, sans-serif'
                    },
                    icon: 'https://maps.google.com/mapfiles/ms/icons/yellow-dot.png'


                }
                   );
            }
            else if (objectMap[i].colorFlag == 4) {
                marker = new google.maps.Marker({
                    position: myLatlng,
                    map: map,
                    animation: google.maps.Animation.DROP,

                    label: {
                        fontFamily: 'Roboto, Arial, sans-serif'
                    },
                    icon: 'https://maps.google.com/mapfiles/ms/icons/red-dot.png'

                }
                );
            }
            else if (objectMap[i].colorFlag == 5) {


                marker = new google.maps.Marker({
                    position: myLatlng,
                    map: map,
                    animation: google.maps.Animation.DROP,

                    label: {
                        fontFamily: 'Roboto, Arial, sans-serif'
                    },
                    icon: 'https://maps.google.com/mapfiles/ms/icons/green-dot.png'

                }
                   );
            }
            else if (objectMap[i].colorFlag == 6) {


                marker = new google.maps.Marker({
                    position: myLatlng,
                    map: map,
                    animation: google.maps.Animation.DROP,

                    label: {
                        fontFamily: 'Roboto, Arial, sans-serif'
                    },
                    icon: 'https://maps.google.com/mapfiles/ms/icons/purple-dot.png'

                }
                );
            }
            else {
                // var pinImage = new google.maps.MarkerImage("https://www.googlemapsmarkers.com/v1/./e9e5dc/");
                var icon = {
                    url: "http://maps.google.com/mapfiles/kml/paddle/wht-circle.png", // url
                    scaledSize: new google.maps.Size(23, 23), // scaled size
                    origin: new google.maps.Point(0, 0), // origin
                    anchor: new google.maps.Point(0, 0) // anchor
                };



                var pinImage = new google.maps.MarkerImage("http://maps.google.com/mapfiles/kml/paddle/wht-circle.png");
                marker = new google.maps.Marker({
                    position: myLatlng,
                    map: map,
                    //  title: data.Street,
                    animation: google.maps.Animation.DROP,

                    label: {
                        // text: installerid,

                        fontFamily: 'Roboto, Arial, sans-serif'
                    },
                    icon: icon,

                }
                );
            }

            latlngbounds.extend(myLatlng);
            // var name = installername;
            //extend the bounds to include each marker's position
            //bounds.extend(marker.position);

            (function (marker, data) {

                //google.maps.event.addListener(marker, 'click', function (e) {
                //google.maps.event.addListener(marker, 'mouseover', function (e) {
                google.maps.event.addListener(marker, 'click', function (e) {

                    infoWindow.setContent(data.street, data.latitude, data.longitude);

                    infoWindow.open(map, marker);


                });

                //google.maps.event.addListener(marker, 'mouseout', function (e) {

                //    infoWindow.close(map, marker);
                //});




                google.maps.event.addListener(marker, 'click', function () {
                    //when the infowindow is open, close it an clear the contents
                    if (contentStringCal == infoWindow.getContent()) {
                        infoWindow.close(map, marker);
                        infoWindow.setContent('');
                    }
                        //otherwise trigger mouseover to open the infowindow
                    else {
                        google.maps.event.trigger(marker, 'mouseover');
                    }
                });

                //clear the contents of the infwindow on closeclick
                google.maps.event.addListener(infoWindow, 'closeclick', function () {
                    infoWindow.setContent('');
                });

            })(marker, data);

            //(function (marker, data) {
            //    google.maps.event.addListener(marker, 'mouseout', function (e) {

            //        infowindow.close();
            //    });
            //})(marker, data);

        }
        //  //now fit the map to the newly inclusive bounds
        // map.fitBounds(bounds);

        //(optional) restore the zoom level after the map is done scaling
        //var listener = google.maps.event.addListener(map, "idle", function () {
        //    map.setZoom(10);
        //    google.maps.event.removeListener(listener);
        //});
        //for (var i in markers) {

        //    var myLatlng = new google.maps.LatLng(markers[i][1], markers[i][2]);
        //    bound.extend(myLatlng);
        //}






        map.fitBounds(latlngbounds);
        map.panToBounds(latlngbounds);

        new google.maps.Rectangle({
            bounds: latlngbounds,
            map: map,
            fillColor: "#000000",
            fillOpacity: 0.0,
            strokeWeight: 0
        });


    }
    else {

        initialize();
    }

    waitingDialog.hide();
}