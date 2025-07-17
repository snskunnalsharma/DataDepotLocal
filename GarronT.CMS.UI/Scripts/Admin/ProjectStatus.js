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


async function initMap() {
    // Request needed libraries.
    const { Map } = await google.maps.importLibrary("maps");
    const { AdvancedMarkerElement, PinElement } = await google.maps.importLibrary(
        "marker",
    );
    var myLatlng = new google.maps.LatLng(39.86911500000000, -100.891880000000) // This is used to center the map to show our markers
    var mapOptions = {
        center: myLatlng,
        zoom: 5,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        marker: false
    };
    map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
    map.setOptions({ minZoom: 3, mapId: "DEMO_MAP_ID", });
    var marker = new google.maps.marker.AdvancedMarkerElement({
        position: myLatlng
    });
}

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
            data: { projectId: Project.val() },
            success: function (data) {
                $('#ddlLocation').empty();
                var count = data.length;
                if (count > 0) {
                    //debugger;
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
    BindProjects();

    $('#ddlInstaller').empty().trigger("chosen:updated");

    $('#ddlformtype').empty().trigger("chosen:updated");

    $('#ulCycle1').empty();
    $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle");

    $('#ulRoute').empty();
    $("#routeHeader").removeClass("ddlHeader").text("Select Route");

    $('#ddlProjectStatus').val("0");
    initialize();
});

function BindProjects() {

    var projectUtilityType = $('#ddlUtilityType').val() == "0" ? "no" :
        ($('#ddlUtilityType').val() == 1 ? "Electric" : ($('#ddlUtilityType').val() == 2 ? "Gas" : "Water"));

    if ($('#dddlInstaller').val() == "0") {
        resetData();
    }

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
            async: true,
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
    //debugger;



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
    var Project = $("#ddlProject option:selected");
    
    $('#ddlInstaller').empty();

    $('#ddlInstaller').trigger("chosen:updated");
  
    $('#ddlformtype').empty();
    $('#ddlformtype').trigger("chosen:updated");

    $('#ulCycle1').empty();
    $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")

    $('#ulRoute').empty();
    $("#routeHeader").removeClass("ddlHeader").text("Select Route")

    initialize();

    //$('#ddlInstaller').empty();
    //$("#ddlInstaller").removeClass("ddlHeader").text("Select Installer")

  //  $('.txtfilterall').val("");
  //  $('.chbSelectAll').prop('checked', false);

   
   
    if (Project.length > 0) {
        BindFormType();
        LocationList();
        CycleList();
        //filterCycle(element);
        filterCycle1();
        getRouteList();
    }

});

$('#ddlLocation').change(function () {
    console.log("Location changes")
    var Project = $("#ddlProject option:selected");
    var Location = $('#ddlLocation option:selected');

    console.log("Project ID = " + Project)
    console.log("Location ID = " + Location)
    /*$('#ddlInstaller').empty();

    $('#ddlInstaller').trigger("chosen:updated");

    $('#ddlformtype').empty();
    $('#ddlformtype').trigger("chosen:updated");

    $('#ulCycle1').empty();
    $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")

    $('#ulRoute').empty();
    $("#routeHeader").removeClass("ddlHeader").text("Select Route")

    initialize();

    //$('#ddlInstaller').empty();
    //$("#ddlInstaller").removeClass("ddlHeader").text("Select Installer")

    //  $('.txtfilterall').val("");
    //  $('.chbSelectAll').prop('checked', false);

*/

    if (Project.length > 0 && Location.length > 0) {
        /*BindFormType();
        LocationList();
        CycleList();
        //filterCycle(element);
        filterCycle1();
        getRouteList();*/
        CycleListByOffice();
    }

});



$('#ddlformtype').change(function () {

    initialize();
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
               // debugger;
                $('#ddlInstaller').empty();

                var count = data.length;
                if (count > 0) {

                    

                    $('#ddlInstaller').append(
                      
                    $('<option></option>').val('0').html("All"))
                    
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
//var Cycle = [];
//$("#ulCycle1 li.ui-selected").each(function () {
//    Cycle.push($(this).text());
//});
//alert(Cycle);

//var Route = [];
//$("#ulroute li.ui-selected").each(function () {
//    Route.push($(this).text());

$('#btnShowMap').click(function () {

    debugger;

    var ProjectID = $("#ddlProject option:selected").val();

    var ddlRoutevalues = [];
    var value = $('#ulroute li.highlighted')
    value.each(function (index, item) { ddlRoutevalues.push($(this).html()) });
    //console.log("root" + ddlRoutevalues);

    var ddlCyclevalues = [];
    var cyclevalue = $('#ulCycle1 li.highlighted')
    cyclevalue.each(function (index, item) { ddlCyclevalues.push($(this).html()) });
    //console.log("cycle = " + ddlCyclevalues);

    /*var Cycle = $("#ulCycle1 li.ui-selected").text();
    alert(Cycle);
    var Route = $("#ulroute li.ui-selected").text(); 
    alert(Route);*/
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
    else if (ddlCyclevalues.length === 0 || ddlCyclevalues === undefined) {
        var $toast = toastr["error"]("Please select a cycle.", "");
    }
    else if (ddlRoutevalues.length === 0 || ddlRoutevalues === undefined) {
        var $toast = toastr["error"]("Please select a route.", "");
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
            GetAddress(ProjectID, 0, FormTypeID, LocationId, ddlCyclevalues, ddlRoutevalues)
        }
     
        else {
            GetAddress(ProjectID, InstallerID, FormTypeID, LocationId, ddlCyclevalues, ddlRoutevalues)
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
$("#btnCycleFilter").click(function () {
    debugger;
    
    waitingDialog.show('Loading Please Wait...');
    initialize();
    $('#ulroute').empty();
    $("#routeHeader").removeClass("ddlHeader").text("Select Route")
    $('#txtfilterRoute').val("");
    $('#Route_SelectAll').prop('checked', false);
    $('#Route1_SelectAll').prop('checked', false);
    var ProjectID = $('#ddlProject').val();
    var InstallerID = $('#ddlInstaller').val();
   // if (InstallerID.length === 0) {
    //    InstallerID = "0";
    //}
    //console.log("InstallerId = " + InstallerID);
    var cyclevalue = [];
    var value = $('#ulCycle1 li.highlighted')
    var selectedlivalue = $('#ulCycle1 li.highlighted').map(function (i, el) { return $(el).val(); });
    var selectedlilength = selectedlivalue.length;

    if (selectedlilength == 0) {
        
        $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
    } else {
        
        $("#CycleHeader").addClass("ddlHeader").text(selectedlilength + " Cycle selected")
    }

    if ($('#ulCycle1 .liCycle:visible').length > 0 && $('#ulCycle1 .highlighted:visible').length == $('#ulCycle1 .liCycle:visible').length) {
        $('#Cycle_SelectAll').prop('checked', true);
    } else {
        $('#Cycle_SelectAll').prop('checked', false);
    }
    //#
    var cyclevalue = [];
    var value = $('#ulCycle1 li.highlighted');
    value.each(function (index, item) { cyclevalue.push($(this).text()); });
    //value.each(function (index, item) { cyclevalue.push($(this).text()); });
    if (cyclevalue != null && cyclevalue.length > 0) {

          
            var urlRootData = $('#GetRootByCycles').val();

            $.ajax({
                url: urlRootData,
                type: 'POST',
                async: true,
                data: { ProjectId: ProjectID, Cycle: cyclevalue, InstallerId: InstallerID},
                success: function (data) {
                    //console.log("Received data:", data);

                    $('#ulroute').empty();

                    $.each(data.Route, function (index, item) { $('#ulroute').append($('<li></li>').val(item).addClass("liRoute active-result").html(item)); });

                  

                    // selectRoute()

                    $('#txtfilterAddress').val("");
                    $('#Address_SelectAll').prop('checked', false);
                    
                    var selectedlivalue = $('#ulroute li.highlighted').map(function (i, el) { return $(el).val(); });

                    

                    var selectedlilength = selectedlivalue.length;
                    if (selectedlilength == 0) {
                        $("#routeHeader").removeClass("ddlHeader").text("Select Route")
                    } else {
                        $("#routeHeader").addClass("ddlHeader").text(selectedlilength + " Route selected")
                    }
                    //#region SelectAll checked/unchecked
                    
                    if ($('#ulroute .liRoute:visible').length > 0 && $('#ulroute .highlighted:visible').length == $('#ulroute .liRoute:visible').length) {
                        
                        $('#Route_SelectAll').prop('checked', true);
                       
                    } else {
                      
                        $('#Route_SelectAll').prop('checked', false);
                    }
                    //#endregion

                    if ($('#ulroute1 .liRoute1:visible').length > 0 && $('#ulroute1 .highlighted:visible').length == $('#ulroute1 .liRoute1:visible').length) {
                       
                        $('#Route1_SelectAll').prop('checked', true);
                    } else {
                       
                        $('#Route1_SelectAll').prop('checked', false);
                    }
                    //#endregion


                    //getAddressList()


                    var ddlRootvalues = [];
                    var value = $('#ulroute li.highlighted')
                    value.each(function (index, item) { ddlRootvalues.push($(this).val()); });


                    waitingDialog.hide();

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
            
            $('#ddlCycle_chosen').removeClass('chosen-with-drop chosen-container-active');
            $('#btnCyclebtn').parent().removeClass('open');

        }
        else {
            waitingDialog.hide();
        }
    
});

// add makhan


$("#btnRouteFilter").click(function () {
  
    initialize();
    var selectedlivalue = $('#ulroute li.highlighted').map(function (i, el) { return $(el).val(); });

    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#routeHeader").removeClass("ddlHeader").text("Select Route");
    } else {
        $("#routeHeader").addClass("ddlHeader").text(selectedlilength + " Route selected");
    }
    //#region SelectAll checked/unchecked
    //if ($('#ulroute .liRoute:visible').length > 0 && $('#ulroute .highlighted:visible').length == $('#ulroute .liRoute:visible').length) {
      //  $('#Route_SelectAll').prop('checked', true);
   // } else {
      //  $('#Route_SelectAll').prop('checked', false);
   // }
    //if ($('#ulroute1 .liRoute1:visible').length > 0 && $('#ulroute1 .highlighted:visible').length == $('#ulroute1 .liRoute1:visible').length) {
    //    $('#Route1_SelectAll').prop('checked', true);
   // } else {
   //     $('#Route1_SelectAll').prop('checked', false);
   // }
    //#endregion


    //getAddressList()
    
    $('#ddlroute_chosen').removeClass('chosen-with-drop chosen-container-active');
    $('#btnRoutebtn').parent().removeClass('open');
})


// end makhan

$(document).ready(function () {
    debugger;

    //    // function to filter cycles
    function filtercycle(input) {
        var filter = $(input).val().touppercase();
        var ul = $("#ulcycle1");
        var li = ul.find("li");
        for (var i = 0; i < li.length; i++) {
            var a = li[i].getelementsbytagname("span")[0];
            if (a.innerhtml.touppercase().indexof(filter) > -1) {
                li[i].style.display = "";
            } else {
                li[i].style.display = "none";
            }
        }
    }
})

//    // Function to filter routes
//    function filterRoute(input) {
//        debugger;

//        var filter = $(input).val().toUpperCase();
//        var ul = $("#ulroute");
//        var li = ul.find("li");
//        for (var i = 0; i < li.length; i++) {
//            var a = li[i].getElementsByTagName("span")[0];
//            if (a.innerHTML.toUpperCase().indexOf(filter) > -1) {
//                li[i].style.display = "";
//            } else {
//                li[i].style.display = "none";
//            }
//        }
//    }

//    // Event listener for cycle and route filter inputs
//    $(".txtfilterall").on("keyup", function () {
//        debugger;

//        var id = $(this).attr("id");
//        if (id === "txtfilterRoute") {
//            filterRoute(this);
//        } else {
//            filterCycle(this);
//        }
//    });

//    // Event listener for cycle filter Ok button
//    $("#btnCycleFilter").on("click", function () {
//            debugger;

//        var urlCycleData = $('#GetCycles').val();
//        $.ajax({
//            url: urlCycleData, 
//            type: "GET",
//            data: { Cycle: Cycle },
//            success: function (response) {
//                console.log(response);
//            },
//            error: function (xhr, status, error) {
//                console.error(xhr.responseText);
//            }
//        });
//    });

//    // Event listener for route filter Ok button
//    $("#btnRouteFilter").on("click", function () {
//        if (check) {
//            if (cyclevalue != null && cyclevalue.length > 0) {
//                var urlRootData = $('#GetRootByCycles').val();
              
//                $.ajax({
//                    url: urlRootData, 
//                    type: "GET",
//                    data: { Route: Route },
//                    success: function (response) {
//                        console.log(response);
//                    },
//                    error: function (xhr, status, error) {
//                        console.error(xhr.responseText);
//                    }
//                });
//            }
//        }
//    });
//});


$("#ulCycle1").selectable({

    start: function (event, ui) {
      
        var target = event.target || event.srcElement;

        if (target.tagName == "LI") {
            //$('#items').trigger('unselected')
            //$('#items li').removeClass('click-selected  highlighted');
            //$(ui.unselected).removeClass('click-selected  highlighted');
        }

       
    },

    selected: function (event, ui) {
        if ($(ui.selected).hasClass('disabledli')) {
            //console.log("in disableli");


        } else {
            if ($(ui.selected).hasClass('highlighted')) {
                $(ui.selected).removeClass('ui-selected click-selected highlighted');

            } else {
                $(ui.selected).addClass('click-selected highlighted');

            }
        }

    },
    unselected: function (event, ui) {
        if ($(ui.selected).hasClass('disabledli')) {

        } else {
            $(ui.unselected).removeClass('click-selected');
        }
    },
    stop: function (event, ui) {

        $('#ulroute').empty();
        $("#routeHeader").removeClass("ddlHeader").text("Select Route")


        //$('#ulAddress').empty();
        //$("#addrHeader").removeClass("ddlHeader").text("Select Street")

        // selectCycle()

    }
});

// add makhan
//#region Route dropdown

$("#ulroute").selectable({
    
    start: function (event, ui) {
        
        var target = event.target || event.srcElement;

        if (target.tagName == "LI") {
            //$('#items').trigger('unselected')
            //$('#items li').removeClass('click-selected  highlighted');
            //$(ui.unselected).removeClass('click-selected  highlighted');
        }
    },
    selected: function (event, ui) {
        if ($(ui.selected).hasClass('disabledli')) {

        } else {
            if ($(ui.selected).hasClass('highlighted')) {
                $(ui.selected).removeClass('ui-selected click-selected highlighted');

            } else {
                $(ui.selected).addClass('click-selected highlighted');

            }
        }

    },
    unselected: function (event, ui) {
        if ($(ui.selected).hasClass('disabledli')) {

        } else {
            $(ui.unselected).removeClass('click-selected');
        }

    },
    stop: function (event, ui) {

        //Address
        //$('#ulAddress').empty();
        //$("#addrHeader").removeClass("ddlHeader").text("Select Street")
        //selectRoute()
    }
});
// end makhan

$('#btnCyclebtn').on('click', function (event) {
    $(this).parent().toggleClass('open');
    $('#btnRoutebtn').parent().removeClass('open');
    $('#btnAddressbtn').parent().removeClass('open');
});

$('#btnRoutebtn').on('click', function (event) {
    $(this).parent().toggleClass('open');
    $('#btnCyclebtn').parent().removeClass('open');
    $('#btnAddressbtn').parent().removeClass('open');
});

function getRouteList() {
   
    // set map location to current city
    //initialize();

    var ProjectID = $('#ddlProject').val();
    var InstallerID = $('#ddlInstaller').val();
    var cyclevalue = [];
    var $spandiv2 = $("#btnRoutebtn");
    $("#RouteDropDownDiv").slideUp("fast");
    $('#MultipleSelectBox_Route').empty();
    var $spandiv2 = $("#btnRoutebtn");
    $spandiv2.children("span").text("Select Route")
    $spandiv2.attr("title", "Select Route")
    var value = $('#ulCycle1 li.highlighted')

    value.each(function (index, item) {
        cyclevalue.push($(this).text())
    });



    if (cyclevalue != null && cyclevalue.length > 0) {

        //GetRootByCycle(value[i].text);

        var urlRootData = $('#GetRootByCycles').val();

        $.ajax({
            url: urlRootData,
            type: 'POST',
            async: true,
            data: { ProjectId: ProjectID, Cycle: cyclevalue, InstallerId: InstallerId},
            complete: function () {
                //waitingDialog.hide();
            },
            success: function (data) {

                $('#ulroute').empty();

                $.each(data.Route, function (index, item) {
                    
                    $('#ulroute').append($('<li></li>').val(item).addClass("liRoute active-result").html(item));
                });

                if (InstallerID != null) {
                    $.each(data.selectedRoute, function (index, item) {
                        if (item.RouteDisabled == false) {
                        
                            $("#ulroute").find("li[value='" + item.RouteId + "']").addClass("ui-selected click-selected highlighted")
                        }
                        else if (item.Appointmentflag != "1") {
                         
                            $("#ulroute").find("li[value='" + item.RouteId + "']").addClass("ui-selected click-selected highlighted")
                        }
                        else {
                           
                            $("#ulroute").find("li[value='" + item.RouteId + "']").addClass("ui-selected click-selected highlighted disabledli")
                        }
                    });
                }
              

                // waitingDialog.hide();
                //selectRoute()

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
};

function selectRoute() {

    $('#txtfilterAddress').val("");
    $('#Address_SelectAll').prop('checked', false);
    var selectedlivalue = $('#ulroute li.highlighted').map(function (i, el) {
        return $(el).val();
    });


    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#routeHeader").removeClass("ddlHeader").text("Select Route")
    } else {
        $("#routeHeader").addClass("ddlHeader").text(selectedlilength + " Route selected")
    }
    //#region SelectAll checked/unchecked
   // if ($('#ulroute .liRoute:visible').length > 0 && $('#ulroute .highlighted:visible').length == $('#ulroute .liRoute:visible').length) {
    //    $('#Route_SelectAll').prop('checked', true);
  //  } else {
   //     $('#Route_SelectAll').prop('checked', false);
   // }

   // if ($('#ulroute1 .liRoute1:visible').length > 0 && $('#ulroute1 .highlighted:visible').length == $('#ulroute1 .liRoute1:visible').length) {
    //    $('#Route1_SelectAll').prop('checked', true);
   // } else {
   //     $('#Route1_SelectAll').prop('checked', false);
   // }
    //#endregion

    //hide route loader
   
}


//#region open/close Route dropdown
$("#ancRoute1_chosen").click(function () {

    debugger;
    if ($("#ddlRoute1_chosen").hasClass('hosen-with-drop chosen-container-active')) {
        $("#ddlroute1_chosen").removeClass("chosen-with-drop chosen-container-active");
    }
    else {
        $("#ddlRoute1_chosen").toggleClass("chosen-with-drop chosen-container-active");
    }

});

$("#ancroute_chosen").click(function () {
    $("#ddladdr_chosen").removeClass("chosen-with-drop chosen-container-active");
    if ($("#ddlroute_chosen").hasClass('chosen-container-active')) {
        $("#ddlroute_chosen").removeClass("chosen-with-drop chosen-container-active");
    }
    else {
        $("#ddlroute_chosen").toggleClass("chosen-with-drop chosen-container-active");
    }

});

//#endregion

function filterRoute(element) {
   
    var value = $(element).val().toLowerCase();

    $("#ulroute > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
   // if ($('#ulroute .liRoute:visible').length > 0 && $('#ulroute .highlighted:visible').length == $('#ulroute .liRoute:visible').length) {
     //   $('#Route_SelectAll').prop('checked', true);
   // } else {
   //     $('#Route_SelectAll').prop('checked', false);
   // }

   // if ($('#ulroute1 .liRoute1:visible').length > 0 && $('#ulroute1 .highlighted:visible').length == $('#ulroute1 .liRoute1:visible').length) {
   //     $('#Route1_SelectAll').prop('checked', true);
   // } else {
   //     $('#Route1_SelectAll').prop('checked', false);
   // }
    //#endregion
}
function filterCycle(element) {

    var value = $(element).val().toLowerCase();

    $("#ulCycle1 > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    // if ($('#ulroute .liRoute:visible').length > 0 && $('#ulroute .highlighted:visible').length == $('#ulroute .liRoute:visible').length) {
    //   $('#Route_SelectAll').prop('checked', true);
    // } else {
    //     $('#Route_SelectAll').prop('checked', false);
    // }

    // if ($('#ulroute1 .liRoute1:visible').length > 0 && $('#ulroute1 .highlighted:visible').length == $('#ulroute1 .liRoute1:visible').length) {
    //     $('#Route1_SelectAll').prop('checked', true);
    // } else {
    //     $('#Route1_SelectAll').prop('checked', false);
    // }
    //#endregion
}

$('#Route_SelectAll').on('click', function () {
   
   // debugger;
   
    //Address
    if ($('#ulroute .liRoute').length > 0) {

        //$('#ulAddress').empty();
        //$("#addrHeader").removeClass("ddlHeader").text("Select Address")

        if (this.checked) {
            $("li.liRoute:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulroute .highlighted').map(function (i, el) { return $(el)[0].value; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#routeHeader").removeClass("ddlHeader").text("Select Route")
            } else {
                $("#routeHeader").addClass("ddlHeader").text(selectedlilength + " Route selected")
            }

        }
        else {
             $("li.liRoute:visible").removeClass('ui-selected click-selected highlighted');
            $("li.liRoute:visible").not('.disabledli').removeClass('ui-selected click-selected highlighted');
            var selectedlitext = $('#ulroute .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#routeHeader").removeClass("ddlHeader").text("Select Route")
            } else {
                $("#routeHeader").addClass("ddlHeader").text(selectedlilength + " Route selected")
            }
        }

        
        //$('#btnShowMap').prop("disabled", true);
        

        //selectRoute()
    }
    //if (InstallerEdit == true) {
    //    $('#btnSave').prop("disabled", false);
  //  }
});

$('#Route1_SelectAll').on('click', function () {
    
    debugger;
    //Address
    if ($('#ulRoute1 .liRoute1').length > 0) {


        if (this.checked) {
            $("li.liRoute1:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulroute1 .highlighted').map(function (i, el) { return $(el)[0].value; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#routeHeader1").removeClass("ddlHeader").text("Select Route")
            } else {
                $("#routeHeader1").addClass("ddlHeader").text(selectedlilength + " Route selected")
            }

        }
        else {
            // $("li.liRoute:visible").removeClass('ui-selected click-selected highlighted');
            $("li.liRoute1:visible").not('.disabledli').removeClass('ui-selected click-selected highlighted');
            var selectedlitext = $('#ulroute1 .highlighted').map(function (i, el) {

                return $(el)[0].value;

            });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#routeHeader1").removeClass("ddlHeader").text("Select Route")
            } else {
                $("#routeHeader1").addClass("ddlHeader").text(selectedlilength + " Route selected")
            }
        }

    }
});


//#region open/close Cycle dropdown
$("#ancCycle_chosen").click(function () {

    if ($("#ddlCycle_chosen").hasClass('chosen-container-active')) {
       // $("#ddlCycle_chosen").toggleClass("chosen-with-drop chosen-container-active")
       // $("#btnCycleFilter").trigger("click");

        $("#ddlCycle_chosen").removeClass("chosen-with-drop chosen-container-active");
    }
    else {
        $("#ddlCycle_chosen").toggleClass("chosen-with-drop chosen-container-active");
    }


});


$("#ancCycle1_chosen").click(function () {
    if ($("#ddlCycle1_chosen").hasClass('chosen-with-drop chosen-container-active')) {
       // $("#ddlCycle_chosen").toggleClass("chosen-with-drop chosen-container-active")
       // $("#btnCycleFilter").trigger("click");
        $("#ddlCycle1_chosen").removeClass('chosen-with-drop chosen-container-active');

    }
    else {
        $("#ddlCycle1_chosen").toggleClass("chosen-with-drop chosen-container-active");
    }


});


//#endregion

function CycleList() {
    var ProjectId = $('#ddlProject').val();
    var urlCycleData = $('#GetCycles').val();
    var selectedlivalue = $('#ulCycle1 li.highlighted').map(function (i, el) { return $(el).val(); });
    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
    } else {
        $("#CycleHeader").addClass("ddlHeader").text(selectedlilength + " Cycle selected")
    }

    var i = 0;
    $.ajax({
        url: urlCycleData,
        async: true,
        data: { ProjectID: ProjectId },
        success: function (Cycle) {
           // console.log("get urlCycleData success")
            $('#ulCycle1').empty();
            var count = Cycle.length;
            for (i = 0; i < count; i++) {
                var cycleVal = Cycle[i];
                console.log("cycle1 = " + cycleVal);
                $('#ulCycle1').append($('<li></li>').val(''+ cycleVal +'').addClass("liCycle active-result").html(Cycle[i]));
            }
            //selectCycle()
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

    /*var value = $(element).val().toLowerCase();

    $("#ulCycle > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });*/

    //#region SelectAll checked/unchecked
    if ($('#ulCycle1 .liCycle:visible').length > 0 && $('#ulCycle1 .highlighted:visible').length == $('#ulCycle1 .liCycle:visible').length) {
        $('#Cycle_SelectAll').prop('checked', true);
    } else {
        $('#Cycle_SelectAll').prop('checked', false);
    }
    //#endregion
}

function CycleListByOffice() {
    var ProjectId = $('#ddlProject').val();
    var OfficeId = $('#ddlLocation').val();
    var urlCycleDataByOffice = $('#GetCyclesByOffice').val();
    /*var selectedlivalue = $('#ulCycle1 li.highlighted').map(function (i, el) { return $(el).val(); });
    var selectedlilength = selectedlivalue.length;
    if (selectedlilength == 0) {
        $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
    } else {
        $("#CycleHeader").addClass("ddlHeader").text(selectedlilength + " Cycle selected")
    }*/
    $("#CycleHeader").addClass("ddlHeader").text("Select Cycle")

    var i = 0;
    $.ajax({
        url: urlCycleDataByOffice,
        async: true,
        data: { ProjectID: ProjectId, LocationID: OfficeId },
        success: function (Cycle) {
            // console.log("get urlCycleData success")
            $('#ulCycle1').empty();
            var count = Cycle.length;
            for (i = 0; i < count; i++) {

                $('#ulCycle1').append($('<li></li>').val(Cycle[i]).addClass("liCycle active-result").html(Cycle[i]));
            }
            //selectCycle()
            $('#Cycle_SelectAll').prop('checked', false);
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

    /*var value = $(element).val().toLowerCase();

    $("#ulCycle > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });*/

    //#region SelectAll checked/unchecked
   /* if ($('#ulCycle1 .liCycle:visible').length > 0 && $('#ulCycle1 .highlighted:visible').length == $('#ulCycle1 .liCycle:visible').length) {
        $('#Cycle_SelectAll').prop('checked', true);
    } else {
        $('#Cycle_SelectAll').prop('checked', false);
    }*/
    //#endregion
}



function filterCycle1(element) {
    //debugger;
    //var value = $(element).val().toLowerCase();

    $("#ulCycle1 > li").each(function () {
        if ($(this).text().toLowerCase().search(value) > -1) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    //#region SelectAll checked/unchecked
    if ($('#ulCycle1 .liCycle1:visible').length > 0 && $('#ulCycle1 .highlighted:visible').length === $('#ulCycle1 .liCycle1:visible').length) {
       
        $('#Cycle1_SelectAll').prop('checked', true);
    } else {
        $('#Cycle1_SelectAll').prop('checked', false);
    }
    //#endregion
}



$('#Cycle_SelectAll').on('click', function () {
    
    if ($('#ulCycle1 .liCycle').length > 0) {
    
        //Route
        $('#ulroute').empty();
        $("#routeHeader").removeClass("ddlHeader").text("Select Route")
       
        if (this.checked) {
            $("li.liCycle:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulCycle .highlighted').map(function (i, el) { return $(el)[0].text; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
            } else {
                $("#CycleHeader").addClass("ddlHeader").text(selectedlilength + " Cycle selected")
            }
        }
        else {
            $("li.liCycle:visible").not('.disabledli').removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulCycle1 .highlighted').map(function (i, el) { return $(el)[0].text; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle")
            } else {
                $("#CycleHeader").addClass("ddlHeader").text(selectedlilength + " Cycle selected")
            }
        }


       
        //$('#btnShowMap').prop("disabled", true);
     

    }

  
});

$('#Cycle1_SelectAll').on('click', function () {

    if ($('#ulCycle1 .liCycle1').length > 0) {
        if (this.checked) {
            $("li.liCycle1:visible").addClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulCycle1 .highlighted').map(function (i, el) { return $(el)[0].text; });
            var selectedlilength = selectedlitext.length;

            if (selectedlilength == 0) {
                $("#CycleHeader1").removeClass("ddlHeader").text("Select Cycle");
            } else {
                $("#CycleHeader1").addClass("ddlHeader").text(selectedlilength + " Cycle selected");
            }
        }
        else {
            $("li.liCycle1:visible").not('.disabledli').removeClass('ui-selected click-selected highlighted');

            var selectedlitext = $('#ulCycle1 .highlighted').map(function (i, el) { return $(el)[0].text; });
            var selectedlilength = selectedlitext.length;
            if (selectedlilength == 0) {
                $("#CycleHeader1").removeClass("ddlHeader").text("Select Cycle");
            } else {
                $("#CycleHeader1").addClass("ddlHeader").text(selectedlilength + " Cycle selected");
            }
        }

    }
});

var GlobalProjectID;
var editInstallerRecord = false;
var existrecordfound;
var FromPopupEdit = false;
var storeAllInstallerCycle = [];


$('#ddlInstaller').unbind().change(function () {
    
    
    // Show loading dialog
    waitingDialog.show('Loading Please Wait...');
    initialize();
    // Get selected ProjectID and InstallerID
    var ProjectID = $('#ddlProject').val();
    var InstallerID = $(this).val();
    var urlCycleData = $('#GetCycledatabyInstaller').val();
    $('#Cycle_SelectAll').prop('checked', false);
    $('#Route_SelectAll').prop('checked', false); 
    if (InstallerID == "0") { 
        
        CycleList(); 
        $('#ulCycle1').empty();
        $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle");
        $('#ulroute').empty();
        $("#routeHeader").removeClass("ddlHeader").text("Select Route");
        $('#Cycle_SelectAll').prop('checked', false); 
        $('#Route_SelectAll').prop('checked', false); 
    } 
    else {
    $.ajax({
        url: urlCycleData,
        data: { "ProjectID": ProjectID, "InstallerId": InstallerID },
        success: function (Cycle) {
            

            $('#ulCycle1').empty();
            $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle");
            $('#ulroute').empty();
            $("#routeHeader").removeClass("ddlHeader").text("Select Route");

            $('#ulCycle1').val();
            $.each(Cycle, function (index, cycleItem) {
                $('#ulCycle1').append($('<li>').val(cycleItem).addClass("liCycle active-result ").html(cycleItem));
            });

            $('#btnShowMap').prop("disabled", false);
            waitingDialog.hide();

            //var selectedCycle = $('#ulCycle1').val();
            //var urlRouteData = $('#GetRouteDatabyCycle').val();

            /*$.ajax({
                url: urlRouteData,
                data: { "ProjectID": ProjectID, "InstallerId": InstallerID, "Cycle": selectedCycle },
                success: function (routeData) {
                    alert("routeData" + routeData);
                    $('#ulroute').empty();
                    $("#routeHeader").removeClass("ddlHeader").text("Select Route");

                    if (Array.isArray(routeData)) {
                        $.each(routeData, function (index, routeItem) {
                            $('#ulroute').append($('<li>').val(routeItem).addClass("liRoute active-result").html(routeItem));
                        });
                    } else {
                        console.error("Route data is not in the expected format:", routeData);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    console.error("Error fetching route data:", textStatus, errorThrown);
                }
            });*/
        },


        error: function (jqXHR, textStatus, errorThrown) {
            waitingDialog.hide();
            console.error("Error fetching cycle data:", textStatus, errorThrown);
        }
    });
    }
    waitingDialog.hide();
});

    /*$('#ddlInstaller').unbind().change(function () {
        // Show loading dialog
        waitingDialog.show('Loading Please Wait...');

        // Get selected ProjectID and InstallerID
        var ProjectID = $('#ddlProject').val();
        var InstallerID = $(this).val();

        // Endpoint to fetch installer data
        var getCheckAddressExistByProjectInstallerDate = $('#GetCheckAddressExistby_ProjectInstallerDates').val();
        var urlCycleData = $('#GetCycledatabyInstaller').val();
        // AJAX request to fetch installer data
        $.ajax({
          type: "GET",
            url: getCheckAddressExistByProjectInstallerDate,
            data: { ProjectID: ProjectID, InstallerID: InstallerID },
            dataType: "json",
            success: function (installerdata) {
                
                $('#ulroute').empty();
                $("#routeHeader").removeClass("ddlHeader").text("Select Route");

         
                $('#ulCycle1').empty();
                $("#CycleHeader").removeClass("ddlHeader").text("Select Cycle");

             
                $('.txtfilterall').val("");
                $('.chbSelectAll').prop('checked', false);
                $('#txtfilterRoute').val("");
                $('#Route_SelectAll').prop('checked', false);
                $('#Route1_SelectAll').prop('checked', false);

                $('#btnShowMap').prop("disabled", true);

                
               // $.ajax({
                    url: urlCycleData,
                    data: { "ProjectID": ProjectID, "InstallerId": InstallerID },
                    success: function (Cycle) {
                        alert("Cycle" + Cycle);

                        $('#ulCycle1').val();
                        $.each(Cycle, function (index, cycleItem) {
                            $('#ulCycle1').append($('<li>').val(cycleItem).addClass("liCycle active-result ").html(cycleItem));
                        });

                        $('#btnShowMap').prop("disabled", false);
                        waitingDialog.hide();

                        var selectedCycle = $('#ulCycle1').val();
                        var urlRouteData = $('#GetRouteDatabyCycle').val();

                        $.ajax({
                            url: urlRouteData,
                            data: { "ProjectID": ProjectID, "InstallerId": InstallerID, "Cycle": selectedCycle },
                            success: function (routeData) {
                                alert("routeData" + routeData);
                                $('#ulroute').empty();
                                $("#routeHeader").removeClass("ddlHeader").text("Select Route");

                                if (Array.isArray(routeData)) {
                                    $.each(routeData, function (index, routeItem) {
                                        $('#ulroute').append($('<li>').val(routeItem).addClass("liRoute active-result").html(routeItem));
                                    });
                                } else {
                                    console.error("Route data is not in the expected format:", routeData);
                                }
                            },
                            error: function (jqXHR, textStatus, errorThrown) {
                                console.error("Error fetching route data:", textStatus, errorThrown);
                            }
                        });
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        waitingDialog.hide();
                        console.error("Error fetching cycle data:", textStatus, errorThrown);
                    }
                });




            },
            error: function (jqXHR, textStatus, errorThrown) {
                waitingDialog.hide(); 
                
                console.error("Error fetching installer data:", textStatus, errorThrown);
               
            }
        });
    });*/




    var objectMap = [
        { latitude: 40.7128, longitude: -74.0060, colorFlag: 1 },
        { latitude: 34.0522, longitude: -118.2437, colorFlag: 0 },
        // Add more data objects as needed
    ];



    // =============== start makhan =============

    async function initialize() {

        //var lat = document.getElementById('txtlat').value;
        //var lon = document.getElementById('txtlon').value;
        //var myLatlng = new google.maps.LatLng(18.5004866, 73.8668996) // This is used to center the map to show our markers
        // Request needed libraries.
        const { Map } = await google.maps.importLibrary("maps");
        const { AdvancedMarkerElement, PinElement } = await google.maps.importLibrary(
            "marker",
        );


        var myLatlng = new google.maps.LatLng(39.86911500000000, -100.891880000000) // This is used to center the map to show our markers
        var mapOptions = {
            center: myLatlng,
            zoom: 05,
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            marker: false
        };

        var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
        map.setOptions({ minZoom: 3, mapId: "DEMO_MAP_ID", });
        var marker = new google.maps.marker.AdvancedMarkerElement({
            position: myLatlng
        });
        //  marker.setMap(map);


    }

document.addEventListener("DOMContentLoaded", function () {
    initialize();
});
    // =============== end makhan =============


    $('#map_canvas').addClass("canvas-map");

    function GetAddress(projectID, InstallerID, FormTypeID, LocationId, ulCycle, ulRoute) {
        //debugger;
        var dd = LocationId;
        //debugger;
        // var projectID = 1;
        var urlGetAddressMap = $('#urlGetAddressMap').val();
        //console.log("In get Address cycle = " + ulCycle);
        //console.log("In get Address route = " + ulRoute);

        $.ajax({
            type: 'POST',
            url: urlGetAddressMap,
            async: true,
            dataType: "json",
            data: { projectID: projectID, InstallerID: InstallerID, formTypeId: FormTypeID, locationId: LocationId, Cycle: ulCycle, Route: ulRoute },
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

var latlongList;
    async function ShowMap(objectMap) {
        debugger;


        const { Map } = await google.maps.importLibrary("maps");

        const { AdvancedMarkerElement, PinElement } = await google.maps.importLibrary(

            "marker",

        );

        // waitingDialog.show('Loading Please Wait...');
        if (objectMap.length > 0) {


            latlongList = objectMap;

            var latlngbounds = new google.maps.LatLngBounds();
            var infoWindow = new google.maps.InfoWindow();

            var mapOptions = {
                center: new google.maps.LatLng(objectMap[0].latitude, objectMap[0].longitude),
                zoom: 06,
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                mapId: "DEMO_MAP_ID",
            };

            var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
           // var installername = $('#ddlInstaller option:selected').text();
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
                    ///addd------------------------------------------------------------------
                    var icon = {
                        url: "https://maps.google.com/mapfiles/ms/icons/blue-dot.png", // url
                        scaledSize: new google.maps.Size(23, 23), // scaled size
                        origin: new google.maps.Point(0, 0), // origin
                        anchor: new google.maps.Point(0, 0) // anchor
                    };
                    ///enddd-------------------------------------------------------------------------------------------------
                    var pinImage = new google.maps.MarkerImage("https://maps.google.com/mapfiles/ms/icons/blue-dot.png");
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

                //        marker = new google.maps.Marker.AdvancedMarkerElement({
                ///            position: myLatlng,
                //            map: map,
                //            animation: google.maps.Animation.DROP,
                //            label: {
                //                fontFamily: 'Roboto, Arial, sans-serif'
                //            },
                //            icon: 'https://maps.google.com/mapfiles/ms/icons/blue-dot.png'
                //
                //       }
                //       );
                //  }
                else if (objectMap[i].colorFlag == 2) {
                    ///addd------------------------------------------------------------------
                    var icon = {
                        url: "https://maps.google.com/mapfiles/ms/icons/orange-dot.png", // url
                        scaledSize: new google.maps.Size(23, 23), // scaled size
                        origin: new google.maps.Point(0, 0), // origin
                        anchor: new google.maps.Point(0, 0) // anchor
                    };
                    ///enddd-------------------------------------------------------------------------------------------------
                    var pinImage = new google.maps.MarkerImage("https://maps.google.com/mapfiles/ms/icons/orange-dot.png");
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


                else if (objectMap[i].colorFlag == 3) {
                    ///addd------------------------------------------------------------------
                    var icon = {
                        url: "https://maps.google.com/mapfiles/ms/icons/yellow-dot.png", // url
                        scaledSize: new google.maps.Size(23, 23), // scaled size
                        origin: new google.maps.Point(0, 0), // origin
                        anchor: new google.maps.Point(0, 0) // anchor
                    };
                    ///enddd-------------------------------------------------------------------------------------------------
                    var pinImage = new google.maps.MarkerImage("https://maps.google.com/mapfiles/ms/icons/yellow-dot.png");
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
                else if (objectMap[i].colorFlag == 4) {
                    ///addd------------------------------------------------------------------
                    var icon = {
                        url: "https://maps.google.com/mapfiles/ms/icons/red-dot.png", // url
                        scaledSize: new google.maps.Size(23, 23), // scaled size
                        origin: new google.maps.Point(0, 0), // origin
                        anchor: new google.maps.Point(0, 0) // anchor
                    };
                    ///enddd-------------------------------------------------------------------------------------------------
                    var pinImage = new google.maps.MarkerImage("https://maps.google.com/mapfiles/ms/icons/red-dot.png");
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
                else if (objectMap[i].colorFlag == 5) {
                    ///addd------------------------------------------------------------------
                    var icon = {
                        url: "https://maps.google.com/mapfiles/ms/icons/green-dot.png", // url
                        scaledSize: new google.maps.Size(23, 23), // scaled size
                        origin: new google.maps.Point(0, 0), // origin
                        anchor: new google.maps.Point(0, 0) // anchor
                    };
                    ///enddd-------------------------------------------------------------------------------------------------

                    var pinImage = new google.maps.MarkerImage("https://maps.google.com/mapfiles/ms/icons/green-dot.png");
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
                else if (objectMap[i].colorFlag == 6) {
                    ///addd------------------------------------------------------------------
                    var icon = {
                        url: "https://maps.google.com/mapfiles/ms/icons/purple-dot.png", // url
                        scaledSize: new google.maps.Size(23, 23), // scaled size
                        origin: new google.maps.Point(0, 0), // origin
                        anchor: new google.maps.Point(0, 0) // anchor
                    };
                    ///enddd-------------------------------------------------------------------------------------------------
                    var pinImage = new google.maps.MarkerImage("https://maps.google.com/mapfiles/ms/icons/purple-dot.png");
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

