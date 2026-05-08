
try {


    var spinner, opts, target;
    var oTable;
    var count = 1;
    var EditMobile;
    $(document).ready(function () {
        
        $("#Administration").addClass('active');
        $("#subAdministration").addClass('block');
        $("#liUserMaster").addClass('active');

        $("#ddlState").chosen({ no_results_text: "Oops, nothing found!", width: 155 });
        $("#ddlCity").chosen({ no_results_text: "Oops, nothing found!", width: 155 });
        $("#ddlGender").chosen({ no_results_text: "Oops, nothing found!", width: 155 });
        $("#ManagerId").chosen({ no_results_text: "Oops, nothing found!", width: 155 });
        $("#FieldSupervisorId").chosen({ no_results_text: "Oops, nothing found!", width: 155 });
        $("#ManagerApprovalRequired").chosen({ no_results_text: "Oops, nothing found!", width: 155 });
        $("#ddlFormType").chosen({ no_results_text: "Oops, nothing found!", width: 155 });
        $("#ddlVehicleMake").chosen({ no_results_text: "Oops, nothing found!", width: 155 });
        $("#ddlVehicleModel").chosen({ no_results_text: "Oops, nothing found!", width: 155 });

        //Bind Role List for ID card
        BindRoleListPo();
        //BindStateList();
        $('#EmployeeCard').hide();
        $("#printButtons").hide();

        //Added on popup
        $("#ddlStateList").chosen({ no_results_text: "Oops, nothing found!" });
        $("#ddlCityList").chosen({ no_results_text: "Oops, nothing found!" });

        $("#ddlRoleListPopup").chosen({ no_results_text: "Oops, nothing found!" });
        $("#ddlCustomer").chosen({ no_results_text: "Oops, nothing found!" });

        var userForm = $("#UserForm").val();
        var userid = $("#EditUserID").val();



        //#region Role dropdown

        $("#ulRole").selectable({

            start: function (event, ui) {

                var target = event.target || event.srcElement;

                if (target.tagName == "LI") {

                }


            },
            selected: function (event, ui) {

                if ($(ui.selected).hasClass('highlighted')) {
                    $(ui.selected).removeClass('ui-selected click-selected highlighted');

                } else {
                    $(ui.selected).addClass('click-selected highlighted');

                }
            },
            unselected: function (event, ui) {

                $(ui.unselected).removeClass('click-selected');
            },
            stop: function (event, ui) {

                selectRole()
                checkinstallerroleselect()


                var objroleList = []

                $('#ulRole li.highlighted').map(function (i, el) { objroleList.push($("#" + $(el)[0].id).text()); });




                if (objroleList != undefined && objroleList.length > 0 && (objroleList.indexOf("Field Supervisor") > -1 || objroleList.indexOf("Manager") > -1)) {

                    if (objroleList.indexOf("Field Supervisor") > -1) {
                        $('#ManagerId').prop('disabled', false).trigger("chosen:updated");
                        $('#FieldSupervisorId').val('0').attr("selected", "selected").prop('disabled', true).trigger("chosen:updated");

                    }

                    if (objroleList.indexOf("Manager") > -1) {
                        $('#ManagerId').val('0').attr("selected", "selected").prop('disabled', true).trigger("chosen:updated");
                        $('#FieldSupervisorId').val('0').attr("selected", "selected").prop('disabled', true).trigger("chosen:updated");
                        $("#ManagerApprovalRequired").val('2').attr("selected", "selected").prop('disabled', true).trigger("chosen:updated");
                    }
                }
                else if (objroleList != undefined && objroleList.length > 0 && (objroleList.indexOf("Installer") > -1 || objroleList.indexOf("Auditor") > -1)) {

                    $('#ManagerApprovalRequired').prop('disabled', false).trigger("chosen:updated");
                    $('#ManagerId').prop('disabled', false).trigger("chosen:updated");
                    $('#FieldSupervisorId').prop('disabled', false).trigger("chosen:updated");

                }
                else {

                    $('#ManagerId').val('0').attr("selected", "selected").prop('disabled', true).trigger("chosen:updated");
                    $('#FieldSupervisorId').val('0').attr("selected", "selected").prop('disabled', true).trigger("chosen:updated");
                }


            }
        });

        //#endregion


        //#region form type dropdown

        $("#ulFormType").selectable({

            start: function (event, ui) {

                var target = event.target || event.srcElement;

                if (target.tagName == "LI") {

                }


            },
            selected: function (event, ui) {

                if ($(ui.selected).hasClass('highlighted')) {
                    $(ui.selected).removeClass('ui-selected click-selected highlighted');

                } else {
                    $(ui.selected).addClass('click-selected highlighted');

                }
            },
            unselected: function (event, ui) {

                $(ui.unselected).removeClass('click-selected');
            },
            stop: function (event, ui) {

                selectFormType()
                checkinstallerroleselect()
            }
        });

        //#endregion





        //#region Spin loading
        $("#loading").fadeIn();
        opts = {
            lines: 12, // The number of lines to draw
            length: 7, // The length of each line
            width: 4, // The line thickness
            radius: 10, // The radius of the inner circle
            color: '#000', // #rgb or #rrggbb
            speed: 1, // Rounds per second
            trail: 60, // Afterglow percentage
            shadow: false, // Whether to render a shadow
            hwaccel: false // Whether to use hardware acceleration
        };
        target = document.getElementById('loading');
        spinner = new Spinner(opts);







        $.validator.addMethod("AlfaNumRegex", function (value, element) {
            return this.optional(element) || /^[0-9a-zA-Z\-\ \s\b]+$/i.test(value);
        }, "must contain only letters, numbers or dashes.");

        $.validator.addMethod("CharacterRegex", function (value, element) {
            return this.optional(element) || /^[a-zA-Z\ \s\b]+$/i.test(value);
        }, "must contain only letters dashes.");

        $.validator.addMethod("PinCodeRegex", function (value, element) {
            return this.optional(element) || /^[0-9]+$/i.test(value);
        }, "must contain only digits");

        $.validator.addMethod("MobileNoRegex", function (value, element) {
            return this.optional(element) || /^\+?\d+(-\d+)*$/i.test(value);
        }, " must contain only numbers");

        $.validator.addMethod("PhoneNoRegex", function (value, element) {
            return this.optional(element) || /^[0-9\-]+$/i.test(value);
        }, "must contain only  numbers.");


        $.validator.addMethod("PasswordRegex", function (value, element) {
            return this.optional(element) || /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z0-9])(?!.*\s).{8,51}$/i.test(value);
        }, "Minimum 6 characters at least alpha,one number and 1 Special Character.");

        $.validator.addMethod("EmailRegex", function (value, element) {
            return this.optional(element) || /^[\w-]+(?:\.[\w-]+)*@(?:[\w-]+\.)+[a-zA-Z]{2,7}$/.test(value);
        }, "Please enter a valid Email ID");



        $('#password, #Confirmpassword').on('keyup', function () {
            if ($('#password').val() == $('#Confirmpassword').val()) {
                $('#message').html('Matching').css('color', 'green');
            } else
                $('#message').html('Not Matching').css('color', 'red');
        });

        //$('#txtPassword').keyup(function () {
        //    $(this).css(this.value.match(/^(?=.*\d)(?=.*[a-zA-Z])(?=.*[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]).{6,}$/));
        //});

        $('.numericOnly').bind('keypress', function (event) {
            $(this).val($(this).val().replace(/^\s+/, ""));
            var regex = new RegExp("^[0-9]+$");
            var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
            if (event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 43 || regex.test(key)) {
                return true;
            }
            return false;
        });


        $('#txtUserFullName').bind('keypress', function (event) {
            $(this).val($(this).val().replace(/^\s+/, ""));
            var regex = new RegExp("^[a-zA-Z\ \s\b]+$");
            var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
            if (event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 43 || regex.test(key)) {
                return true;
            }
            return false;
        });


        $.validator.setDefaults({ ignore: ":hidden:not(.chosen-select)" })

        var validator = $('#form3').validate({
            onkeyup: false,
            debug: true, onsubmit: false,
            errorClass: "authError error",
            rules: {
                UserFullName: {
                    required: true,
                    maxlength: 200,
                    AlfaNumRegex: true,
                    CharacterRegex: true,
                },
                LogInId: {
                    required: true,
                    maxlength: 200,
                    //AlfaNumRegex: true,
                },
                Password: {
                    required: true,
                    PasswordRegex: true,
                },
                Confirmpassword: {
                    required: true,
                    equalTo: "#txtPassword"
                },
                MobileNo: {
                    required: true,
                    maxlength: 15,
                    minlength: 10,
                    MobileNoRegex: true,
                },

                Email: {
                    required: true,
                    maxlength: 100,
                    EmailRegex: true
                },
                PrimaryRole: {
                    required: false,
                },
                StateID: {
                    required: false,
                },
                CityID: {
                    required: false,
                },
                Gender: {
                    required: false,
                },
                FormType: {
                    required: false,
                },

            },
            messages: {
                UserFullName: {
                    required: "Name Required",
                    maxlength: "Maxlength 200 characters.",
                    AlfaNumRegex: "must contain only letters",
                    CharacterRegex: "must contain only letters"
                },
                LogInId: {
                    required: "Login ID Required",
                    maxlength: "Maxlength 200 characters.",
                    // AlfaNumRegex: "Description must contain only letters, numbers or dashes"

                },
                Password: {
                    required: "Password Required",
                    AlfaNumRegex: "Password Required",
                    PasswordRegex: "<br>Minimum 8 characters <br>atleast 1 alphabet,<br>1 number and 1 Special Character."

                },
                Confirmpassword: {
                    required: "Re-enter Password",
                    equalTo: "Password Not Match",
                },
                MobileNo: {
                    required: "Mobile No. Required",
                    maxlength: "Maxlength 15.",
                    minlength: "Minlength 10.",
                    MobileNoRegex: "Mobile Number must contain only  numbers."
                },
                PrimaryRole: {
                    required: "User Role Required",
                    AlfaNumRegex: "Role Required"
                },

                Email: {
                    required: "Email-ID Required.",
                    maxlength: "Maxlength 50 characters.",
                    EmailRegex: "Please enter a valid Email ID"
                },
                StateID: {
                    required: "Select State.",

                },
                CityID: {
                    required: "Select City.",

                },
                Gender: {
                    required: "Select Gender.",

                },
                FormType: {
                    required: "Select Form Type.",

                },
            },

            submitHandler: function (form) {

                //EditRecord();
                //$("#element_to_pop_up").dialog("close");
                //return false;
            },
            invalidHandler: function () {


            }
        });



        $("#btnCancel").click(function () {

            validator.resetForm();
            $("#myModal").modal('hide');
            ClearUser();
            RemoveErrorClass();
            TmplistClear();
        })


        $("#btnReset").click(function () {

            //  $('#ddlCity').val(0).attr("selected", "selected");
            validator.resetForm();
            $('#txtUserFullName').focus();
            // $('#ddlPrimaryRole').prop('disabled', true);
            RemoveErrorClass();
            ClearUser();
            // ClearContactClient();


            TmplistClear();
            //
            // TmpListRemove(0);
            // 
            // count = 1;

        })





        

        BindManagerList();
        // BindSupervisorList();
        if (userForm == "Create") {

            bindServices();
            ClearUser();
        }
        else {

            ClearUser();


            //  GetRole();
            // GetState()
        }

        if (userid != undefined) {

            Edit(userid)
        }



        $("#btnAddUser").click(function () {


            // $('#ddlState').prop('disabled', true);
            // $('#ddlCity').prop('disabled', false);
            $("#chkShowDeactivate").prop('checked', false);
            $("#btnReset").show();
            $("h4.modal-title").text("Add New User");
            validator.resetForm();
            $('#myModal').on('shown.bs.modal', function () {
                $('#txtUserFullName').focus();
            })
            TmpListRemove(0);
            $("#btnReset").show();
            RemoveErrorClass();
            ClearUser();
            // ClearContactClient();
            //$('#ddlState').empty();
            count = 1;

        })
        $(".allocation_m").addClass("allocation_usr");



        $("#btnPrintIDCard").click(function () {
            


            document.getElementById("ddlStateList").options.length = 0;
            document.getElementById("ddlCityList").options.length = 0;
            document.getElementById("ddlCustomer").options.length = 0;
            document.getElementById("ddlRoleListPopup").options.length = 0;
            CKEDITOR.instances['txtMessage'].setData('');


            $('#EmployeeCard').hide();
            var content = $("#txtUserFullName").val();



            $("#lblEmployeeName").text($("#txtUserFullName").val());

            $("#lblEmpName").text($("#txtUserFullName").val());
            $("#lblEmpName2").text($("#txtUserFullName").val());


            $("#lblEmployeeVehicle").text($('#ddlVehicleMake option:selected').text());
            $("#lblEmployeeVehicleType").text($('#ddlVehicleModel option:selected').text());
            $("#lblEmployeeVehicleColor").text($("#txtColor").val());
            $("#lblEmployeeLicenceNo").text($("#txtLicensePlateNumber").val());



        });


    });


    $("#ddlRoleListPopup").change(function () {
        
        ClearICard();


        $('#ddlCustomer').val('0').attr("selected", "selected").trigger("chosen:updated");

        $('#ddlStateList').empty().trigger("chosen:updated");

        $('#ddlCityList').empty().trigger("chosen:updated");

        //$('#ManagerId').empty().trigger("chosen:updated");

        //$('#SupervisorId').empty().trigger("chosen:updated");



        var roleid = $('#ddlRoleListPopup option:selected').val();

        if (roleid != undefined && roleid != "0") {

            BindCustomerListPo();
            //urlGetManagerList

        }


    });


    $("#ddlCustomer").change(function () {
        ClearICard();
        var customerId = $('#ddlCustomer option:selected').val();

        if (customerId != undefined && customerId != "0") {

            BindStateList(customerId);

        }
        $('#ddlCityList').empty();
        $('#ddlCityList').trigger("chosen:updated");

    });


    $('#ddlStateList').change(function () {

        

        var stateid = $('#ddlStateList option:selected').val();

        var customerId = $('#ddlCustomer option:selected').val();

        if (stateid != undefined && stateid != "0") {

            BindCityList(customerId, stateid);
        }

        ClearICard();
    });


    $("#ddlCityList").change(function () {

        ClearICard();
        
        var cityid = $('#ddlCityList option:selected').val();
        if (cityid != undefined && cityid != "0") {


            var UserId = $('#EditUserID').val();
            var RoleId = $('#ddlRoleListPopup option:selected').val();
            var customerId = $('#ddlCustomer option:selected').val();
            var stateId = $('#ddlStateList option:selected').val();
            var cityId = $('#ddlCityList option:selected').val();

            if (RoleId != undefined && RoleId != '' &&
                customerId != undefined && customerId != '' &&
                stateId != undefined && stateId != '' &&
                cityId != undefined && cityId != ''
                ) {


                var urlGetLogoMessage = $('#urlGetLogoMessage').val();
                waitingDialog.show('Loading Please Wait...');
                $.ajax({
                    type: 'POST',
                    url: urlGetLogoMessage,
                    async: true,
                    data: { "UserId": UserId, "RoleId": RoleId, "customerId": customerId, "stateId": stateId, "cityId": cityId },
                    success: function (data) {
                        
                        CKEDITOR.instances['txtMessage'].setData(data.imageMessage);
                        waitingDialog.hide();
                    },
                    error: function (error) {
                        console.log(error)
                        waitingDialog.hide();
                    }

                });
            }
            else {

                ClearICard();
            }
        }
        ClearICard();
    });

    function ClearICard() {

        $("#EmployeeCard").css("display", "none");
        $("#printButtons").css("display", "none");



        $("#DivMessage").html('');
        $("#lblRoleName").text($('#ddlRoleListPopup option:selected').text());
        $("#lblMyRole").text($('#ddlRoleListPopup option:selected').text());
        $("#EmpolyeeProfileImg").attr('src', '');
        $("#EmpolyeeProfileImg2").attr('src', '');
        $("#CityLogoImg").attr('src', '');
        $("#CityLogoImg2").attr('src', '');
        $("#CMSLogoImage").attr('src', '');
        $("#CMSLogoImage2").attr('src', '');

        $("#lblEmployeeName").text('');
        $("#lblEmpName").text('');
        $("#lblEmpName2").text('');
        $("#lblEmployeeVehicle").text('-');
        $("#lblPrtEmployeeVehicle").text('-');
        $("#lblEmployeeVehicleType").text('-');
        $("#lblPrtEmployeeVehicleType").text('-');

        $("#lblEmployeeVehicleColor").text('-');
        $("#lblPrtEmployeeVehicleColor").text('-');
        $("#lblEmployeeLicenceNo").text('-');
        $("#lblPrtEmployeeLicenceNo").text('-');

        $("#DivMessage").html('');
        CKEDITOR.instances['txtMessage'].setData('');


    }

    $("#btnPreview").click(function () {
        

        var cityName = $('#ddlCityList option:selected').text();

        if (cityName != undefined && cityName != '') {



            var customerId = $('#ddlCustomer option:selected').val();
            var stateId = $('#ddlStateList option:selected').val();
            var cityId = $('#ddlCityList option:selected').val();

            //GetUserProfileImage(a);
            GetCityLogoByImageName(cityId, customerId);
            // GetCityLogoImage(imageName);

            GetCMsLogo();

            var value = CKEDITOR.instances['txtMessage'].getData();

            if (value == '' || value == 'undefined') {

                var $toast = toastr["error"]('Please enter the message  first', 'Notification');
                return;
            }
            else {
                var size = value.length;
                if (size > 250) {
                    var $toast = toastr["error"]('Please enter information up to 250 characters.', 'Notification');
                    return;
                }


                $("#DivMessage").html(value);
                $("#lblRoleName").text($('#ddlRoleListPopup option:selected').text());
                $("#lblMyRole").text($('#ddlRoleListPopup option:selected').text());


                //$block.css({ 'height': 95 + '%', 'min-height': 50 + '%' });
                // $('modal-lg').css()         

                $("#lblEmployeeName").text($("#txtUserFullName").val());
                $("#lblEmpName").text($("#txtUserFullName").val());
                $("#lblEmpName2").text($("#txtUserFullName").val());

                var vehicleMake = $('#ddlVehicleMake option:selected').text();
                var vehicleModel = $('#ddlVehicleModel option:selected').text();

                if (vehicleModel === undefined || vehicleModel === null || vehicleModel.length == 0) {
                    $("#lblEmployeeVehicleType").text('-');
                    $("#lblPrtEmployeeVehicleType").text('-');
                }
                else {
                    $("#lblEmployeeVehicleType").text(vehicleMake + " - " + vehicleModel);
                    $("#lblPrtEmployeeVehicleType").text(vehicleMake + " - " + vehicleModel);
                }

                var color = $("#txtColor").val();

                if (color === undefined || color === null || color.length === 0) {
                    $("#lblEmployeeVehicleColor").text('-');
                    $("#lblPrtEmployeeVehicleColor").text('-');
                }
                else {
                    $("#lblEmployeeVehicleColor").text(color);
                    $("#lblprtemployeevehiclecolor").text(color);
                }

                var licenceNo = $("#txtLicensePlateNumber").val();

                if (licenceNo === undefined || licenceNo === null || licenceNo.length === 0) {
                    $("#lblEmployeeLicenceNo").text('-');
                    $("#lblprtemployeelicenceno").text('-');
                }
                else {
                    $("#lblEmployeeLicenceNo").text(licenceNo);
                    $("#lblprtemployeelicenceno").text(licenceNo);
                }

                $('#EmployeeCard').show();
                $("#printButtons").show();


                // long UserId, string RoleId, long LogoId, long stateId, long cityId, string imageMessage
                var UserId = $('#EditUserID').val();
                var RoleId = $('#ddlRoleListPopup option:selected').val();

                var imageMessage = value;

                var urlSaveLogoMessage = $('#urlSaveLogoMessage').val();
                waitingDialog.show('Loading Please Wait...');
                $.ajax({
                    type: 'POST',
                    url: urlSaveLogoMessage,
                    async: false,
                    data: { "UserId": UserId, "RoleId": RoleId, "customerId": customerId, "stateId": stateId, "cityId": cityId, "imageMessage": imageMessage },
                    success: function (data) {
                        
                        waitingDialog.hide();

                        if (data.profileImage == null || data.profileImage == undefined || data.profileImage == "") {
                            data.profileImage = "https://www.placehold.it/200x150/EFEFEF/AAAAAA&amp;text=no+image";
                        }
                        else {
                            data.profileImage = $('#WebURL').val() + data.profileImage;
                        }

                        $("#EmpolyeeProfileImg").attr('src', '');
                        $("#EmpolyeeProfileImg2").attr('src', '');
                        $("#EmpolyeeProfileImg").attr('src', data.profileImage);
                        $("#EmpolyeeProfileImg2").attr('src', data.profileImage);
                    },
                    error: function (error) {
                        console.log(error)
                        waitingDialog.hide();
                    }

                });

            }
        }
        else {
            var $toast = toastr["error"]('Please select the image name.', 'Notification');

        }



    });

    function GetCMsLogo() {
        
        var path = 'logo_03_1.png'
        var Webpath = $('#WebURL').val();
        // var CmsimagePath = window.location.protocol + "//" + window.location.host + "/" + 'images/' + path;
        var CmsimagePath = Webpath + 'images/' + path;
        $("#CMSLogoImage").attr('src', '');
        $("#CMSLogoImage").attr('src', CmsimagePath);
        $("#CMSLogoImage2").attr('src', '');
        $("#CMSLogoImage2").attr('src', CmsimagePath);

        $("#CMSLogoImage").attr('style', "height: 50px;width: 100px;");
        $("#CMSLogoImage2").attr('style', "height: 50px;width: 100px;");
        //style="height: 50px;width: 100px;"

    }

    function GetCityLogoByImageName(cityId, customerId) {
        
        var urlGetCityLogoName = $("#urlGetClientLogoImagePath").val();

        $.ajax({
            type: 'GET',
            data: { cityId: cityId, customerId: customerId },
            async: false,
            url: urlGetCityLogoName,
            success: function (data) {
                
                if (data.LogoName != '' && data.LogoName != null) {
                    var Imagepath = data.LogoName;
                    var ImageName = data.ImageName;
                    var path = $('#WebURL').val();

                    //var newURL = window.location.protocol + "//" + window.location.host + "/" + window.location.pathname;

                    imagePath = path + 'Uploads/UploadCityLogo/' + ImageName + "/" + Imagepath;

                    $("#CityLogoImg").attr('src', '');
                    $("#CityLogoImg").attr('src', imagePath);
                    $("#CityLogoImg2").attr('src', '');
                    $("#CityLogoImg2").attr('src', imagePath);

                    // waitingDialog.hide();
                    // var $toast = toastr["success"]("Image name ", "Success Notification !");
                }
                else {

                    //waitingDialog.hide();
                    // var $toast = toastr["error"]("Image logo not Uploaded.", "Error Notification !");
                }
            },
            error: function (error) {
                return false;
                console.log(error)
                //alert("something seems wrong");
            }
        });

    }

    function BindRoleListPo() {

        var urlRoleData = $('#urlRoleList').val();

        $.ajax({

            type: 'GET',

            url: urlRoleData,

            success: function (data) {

                
                $('#ddlRoleListPopup').empty();

                $.each(data, function (key, value) {

                    if (key == 0) {

                        $('#ddlRoleListPopup').append($('<option></option>').val("").html(""));

                    }

                    $('#ddlRoleListPopup').append($('<option></option>').val(value.Id).html(value.Name)

                    );

                });

                $('.chosen-select').trigger('chosen:updated');

            },
            error: function (error) {

                console.log(error)
                //alert("something seems wrong");
            }
        });

    }

    function BindCustomerListPo() {

        var urlCustomerListData = $('#urlGetCustomerList').val();
        $.ajax({

            type: 'GET',

            url: urlCustomerListData,

            success: function (data) {

                
                $('#ddlCustomer').empty();

                $.each(data, function (key, value) {
                    if (key == 0) {

                        $('#ddlCustomer').append($('<option></option>').val("").html(""));

                    }

                    $('#ddlCustomer').append($('<option></option>').val(value.CustomerID).html(value.CustomerName));

                });

                $('.chosen-select').trigger('chosen:updated');

            },
            error: function (error) {
                console.log(error)
                //alert("something seems wrong");
            }
        });

    }

    function BindSupervisorList() {

        


    }

    function BindManagerList(projectIds, selectedManagerId) {

        
        var urlManager = $('#urlGetManagerList').val();
        var postData = { "RoleName": "Manager" };
        var editUserId = $('#EditUserID').val();
        if (editUserId != undefined && editUserId != null && editUserId != "") {
            postData.UserId = editUserId;
        }

        if (projectIds != undefined && projectIds != null && projectIds.length != undefined && projectIds.length > 0) {
            var urlFiltered = $('#urlGetManagerListByProjects').val();
            if (urlFiltered != undefined && urlFiltered != null && urlFiltered != "") {
                urlManager = urlFiltered;
                postData.ProjectIds = projectIds.join(',');
            }
        }

        $.ajax({

            type: 'POST',
            async: false,
            data: postData,

            url: urlManager,

            success: function (data) {

                

                $('#ManagerId').empty();

                $.each(data, function (key, value) {

                    if (key == 0) {

                        $('#ManagerId').append($('<option></option>').val("").html(""));
                    }
                    $('#ManagerId').append($('<option></option>').val(value.ChildId).html(value.ChildName));

                });

                if (selectedManagerId != undefined && selectedManagerId != null && selectedManagerId != "") {
                    $('#ManagerId').val(selectedManagerId);
                }
                $('#ManagerId').trigger("chosen:updated");
            },
            error: function (error) {
                console.log(error)
            }

        });

    }

    function BindStateList(customerId) {

        
        var urlStateData = $('#urlGetStateListURL').val();

        $.ajax({

            type: 'POST',

            data: { "customerId": customerId },

            url: urlStateData,

            success: function (data) {

                

                $('#ddlStateList').empty();

                $.each(data, function (key, value) {

                    if (key == 0) {

                        $('#ddlStateList').append($('<option></option>').val("").html(""));
                    }
                    $('#ddlStateList').append($('<option></option>').val(value.StateID).html(value.StateName));

                });

                $('.chosen-select').trigger('chosen:updated');
            },
            error: function (error) {

                console.log(error)

            }

        });

    }

    function BindCityList(customerId, stateid) {

        

        var urlCityData = $('#urlGetCityLisyByStateIdURL').val();

        $.ajax({

            type: 'GET',

            data: { "stateId": stateid, "customerId": customerId },

            url: urlCityData,

            success: function (data) {

                $('#ddlCityList').empty();

                $.each(data, function (index, item) {
                    if (index == 0) {

                        $('#ddlCityList').append($('<option></option>').val('').html(""));

                    }

                    $('#ddlCityList').append($('<option></option>').val(item.CityID).html(item.CityName));
                });

                $('.chosen-select').trigger('chosen:updated');
            },

        });
    }

    $("#btnPrintCancel").click(function () {


        ClearePopup();


    });

    $("#btnPrintsID").click(function () {
        
        var roleid = $('#ddlRoleListPopup option:selected').val();
        var stateyid = $('#ddlStateList option:selected').val();
        var cityid = $('#ddlCityList option:selected').val();
        var customerId = $('#ddlCustomer option:selected').val();
        if (roleid.length == "0" || roleid == undefined) {

            var $toast = toastr["error"]('Please select role first', 'Notification');
            return;
        }
        else if (customerId.length == "0" || customerId == undefined) {
            var $toast = toastr["error"]('Please select customer name first', 'Notification');
            return;
        }
        else if (stateyid.length == "0" || stateyid == undefined) {

            var $toast = toastr["error"]('Please select state first', 'Notification');
            return;
        }
        else if (cityid.length == "0" || cityid == undefined) {

            var $toast = toastr["error"]('Please select City first', 'Notification');
            return;
        }

        var value = CKEDITOR.instances['txtMessage'].getData();
        $("#DivPrintMessage").html(value);


        $("#lblPrtEmployeeName").text($("#txtUserFullName").val());
        $("#lblPrtRoleName").text($("#lblRoleName").text());
        $("#lblPrtMyRole").text($("#lblRoleName").text());
        $("#lblPrtMyRole2").text($("#lblRoleName").text());
        $("#lblPrtEmpName").text($("#txtUserFullName").val());
        $("#lblPrtEmpName2").text($("#txtUserFullName").val());

        var contents = document.getElementById("EmployeeCard2").innerHTML;


        var frame1 = $('<iframe />');
        frame1[0].name = "frame1";
        frame1.css({ "position": "absolute", "top": "-1000000px" });
        $("body").append(frame1);
        var frameDoc = frame1[0].contentWindow ? frame1[0].contentWindow : frame1[0].contentDocument.document ? frame1[0].contentDocument.document : frame1[0].contentDocument;
        frameDoc.document.open();
        //Create a new HTML document.
        frameDoc.document.write('<!DOCTYPE html>');
        frameDoc.document.write('<html xmlns="https://www.w3.org/1999/xhtml" xml:lang="en">');
        frameDoc.document.write('<head><title>Print</title>');
        frameDoc.document.write('<style type="text/css">body{ margin:0px auto; ');
        frameDoc.document.write('font-family:Verdana, Arial;color:#000;');
        frameDoc.document.write('font-family:Verdana, Geneva, sans-serif; font-size:12px;}');
        frameDoc.document.write('a{color:#000; text-decoration:none;} </style>');
        frameDoc.document.write('</head><body style="border:2px solid black;width:70%;" onLoad="self.print()" >'); //
        //frameDoc.document.write('</head><body>');
        //Append the external CSS file.
        //frameDoc.document.write('<link href="" rel="stylesheet" type="text/css" />');
        //Append the DIV contents.
        frameDoc.document.write(contents);
        frameDoc.document.write('</body></html>');
        frameDoc.document.close();
        setTimeout(function () {
            window.frames["frame1"].focus();
            window.frames["frame1"].print();
            frame1.remove();
        });   //, 500);
    });

    function ClearePopup() {
        

        $('#EmployeeCard').hide();



        $('#ddlRoleListPopup').val("0").trigger('chosen:updated');
        $('#ddlCustomer').val("0").trigger('chosen:updated');
        $('#ddlStateList').val("0").trigger('chosen:updated');
        $('#ddlCityList').val("0").trigger('chosen:updated');


        CKEDITOR.instances['txtMessage'].setData('');
        $("#lblEmployeeName").text('');
        $("#lblRoleName").text('');
        $("#lblMyRole").text('');
        $("#lblMyRole2").text('');
        $("#lblEmpName").text('');
        $("#lblEmpName2").text('');
        $("#printButtons").hide();



    }

    function checkRoleStatus() {
        var result = true;
        var meterTypeList = $('#ddlPrimaryRole').val();
        if (meterSizeList == null || meterSizeList.length == 0) {

            var $toast = toastr["error"]('Please select Role', 'Error Notification');
            result = false;
        }
        return result;

    }

    function TmpListRemove(id) {

        var urlGetData = $('#TmpListRemoveUrl').val();
        $.ajax({
            type: 'POST',
            url: urlGetData,
            //contentType: "application/json; charset=utf-8",
            //dataType: "json",
            data: { id: id },
            success: function (msg) {

                oTable = $('#dynamic-tableCategory').dataTable(
       {
           "bInfo": false,
           "bPaginate": false,
           "bFilter": false,
           "bLengthChange": false,
           "bDestroy": true,
           "bserverSide": true,
           "bDeferRender": true,
           //"sAjaxDataProp": 'aData',
           "oLanguage": {
               "sLengthMenu": '_MENU_ Records per page'
           },
           //"sAjaxSource": msg.result,
           "aaData": msg,
           "aoColumns": [
                              { "mData": "UserID" },
                             { "mData": "FirstName" },
                             { "mData": "LastName" },
                             { "mData": "MobileNumber" },
                             { "mData": "Address" },
                             { "mData": "EmailId" },
                               { "bSearchable": false, "mData": null },
           ],
           "aoColumnDefs": [

                                                  {
                                                      "mRender": function (data, type, full, row) {

                                                          // return '<a href="#confirmDeactivatedialog1" data-backdrop="static" data-toggle="modal" id=' + full.MobileNumber + ' onclick="Deactivate1(' + full.MobileNumber + ');" class="btn btn-danger btn-xs" role="button" alt="Remove" title="Remove"><i class="fa fa-ban"></i></a>';
                                                          return '<a href="#" id=' + full.UserID + ' onclick="EditContactDetails(' + full.UserID + ');" class="btn btn-primary btn-xs" role="button" alt="Edit" title="Edit"><i class="fa fa-pencil"></i></a><a href="#confirmDeactivatedialog1" data-backdrop="static" data-toggle="modal" id=' + full.UserID + ' onclick="Deactivate1(' + full.UserID + ');" class="btn btn-danger btn-xs" role="button" alt="Remove" title="Remove"><i class="fa fa-ban"></i></a>';

                                                      },

                                                      "aTargets": [5]
                                                  },
          { "bVisible": false, "aTargets": [0] },
           ],

       }
    )
                spinner.stop(target);


            }

        });
        // spinner.stop(target);
    }

    function TmplistClear() {
        var uri = $('#TmpListClearUrl').val();

        $.ajax({
            url: uri,
            //data: $("#form3").serialize(),
            type: 'POST',
            //dataType: 'json',
            //data: JSON.stringify(AttributeModel),
            //contentType: "application/json; charset=utf-8",
            success: function (msg) {
                spinner.stop(target);
            }
        })
        spinner.stop(target);
    }

    function GetRole() {

        var urlRoleData = $('#urlRoleList').val();
        $.ajax({
            type: 'GET',
            url: urlRoleData,
            //   contentType: "application/json; charset=utf-8",
            // dataType: "json",
            async: false,
            success: function (data) {

                $('#ddlPrimaryRole').empty();
                $("#RoleHeader").removeClass("ddlHeader").text("Select Role")
                $("#Role_SelectAll").prop("checked", false);
                $('#ulRole').empty();

                StateData = data;
                $.each(data, function (index, item) {

                    $('#ulRole').append($('<li></li>').attr("Id", item.Id).val(item.Id).addClass("liRole active-result").html(item.Name));

                    $('#ddlPrimaryRole').append(
                           $('<option></option>').val(item.Id).html(item.Name)
                     );
                });

                //$('#ddlPrimaryRole').multiselect(
                //    {
                //        includeSelectAllOption: true,
                //        enableFiltering: true,
                //        enableCaseInsensitiveFiltering: true,
                //        onChange: function (element, checked) {

                //            var brands = $('#ddlPrimaryRole option:selected');
                //            var selected = [];
                //            $('.InstallerRoleDiv').hide()
                //            var installerfound = false;
                //            $(brands).each(function (index, brand) {
                //                if ($(this).text() == "Installer") {
                //                    installerfound = true
                //                    $('.InstallerRoleDiv').show();
                //                }
                //            });
                //            if (installerfound == false) {
                //                ClearInstaller()
                //            }


                //        },

                //    })


                //$("#ddlPrimaryRole").multiselect("refresh");



                $(".allocation_m").addClass("allocation_usr");

                spinner.stop(target);
            },
            error: function (error) {
                console.log(error)
                //alert("something seems wrong");
            }
        });
        //spinner.stop(target);
    }




    function RemoveErrorClass() {
        $("#txtLogInId").removeClass("error");
        $("#txtLogInId-error").remove();
        $("#txtLogInId-error").val('');

        $("#txtEmail").removeClass("error");
        $("#txtEmail-error").remove();
        $("#txtEmail-error").val('');

        $("#txtPassword").removeClass("error");
        $("#txtPassword-error").remove();
        $("#txtPassword-error").val('');

        $("#txtUserFullName").removeClass("error");
        $("#txtUserFullName-error").remove();
        $("#txtUserFullName-error").val('');

        $("#txtMobileNo").removeClass("error");
        $("#txtMobileNo-error").remove();
        $("#txtMobileNo-error").val('');

        $("#txtConfirmPassword").removeClass("error");
        $("#txtConfirmPassword-error").remove();
        $("#txtConfirmPassword-error").val('');

        $("#txtUserAddress").removeClass("error");
        $("#txtUserAddress-error").remove();
        $("#txtUserAddress-error").val('');

        $("#txtUserFullName").removeClass("error");
        $("#txtUserFullName-error").remove();
        $("#txtUserFullName-error").val('');

    }

    function ClearUser() {
        $('#profileimgremove').hide();
        $('#vehicleimgremove').hide();
        $("#txtLogInId").val('');
        $("#txtEmail").val('');
        $("#txtPassword").val('');
        // $("#txtClientName").val('');
        $("#txtMobileNo").val('');
        $("#txtConfirmPassword").val('');
        $("#txtUserAddress").val('');
        $("#txtuserfullname").val('');

        $("#ddlState").empty();
        $("#ddlCity").empty();
        $("#txtZip").val('');
        $("#txtHomePhoneNumber").val('');
        $("#txtHomePhoneNumber").val('');
        $("#txtJobType").val('');

        $(".btnRemoveimage").trigger("click");


        $("#MainImgLOgo").attr('src', '');


        $('.InstallerRoleDiv').hide()
        $('.chosen-select').trigger('chosen:updated');
        $('#ddlGender').val('').trigger('chosen:updated');


        $(".serviceinput").val('');
        GetRole();

        GetState()

        GetFormTypeList()
        ClearInstaller()
        GetVehicleMakeList()
    }

    function ClearInstaller() {

        $("#txtMake").val('');
        $("#txtModel").val('');
        $("#txtYear").val('');
        $("#txtLicensePlateNumber").val('');
        $("#txtVehicle").val('');
        $("#txtColor").val('');


    }


    function EditRecord() {
        
        //var objroleList = $('#ddlPrimaryRole').val();

        //alert(profilepicfolder);
        //alert(vehiclepicfolder);

        var objroleList = [];
        var cyclevalue = $('#ulRole li.highlighted')
        cyclevalue.each(function (index, item) {

            objroleList.push($(this)[0].id)

        });

        var objFormTypeList = [];
        var FormTypevalue = $('#ulFormType li.highlighted')
        FormTypevalue.each(function (index, item) {

            objFormTypeList.push($(this).val())

        });

        //objroleList = $('#ulRole li.highlighted').map(function (i, el) {
        //    return $(el)[0].id;
        //});

        var uri = $('#urlEditData').val();
        var model = $("#form3").serializeArray();

        var vehicle = $("#vehicleform").serializeArray();


        var $InnerDivs = document.getElementsByClassName('serviceInnerDiv');
        var ServiceRatesList = [];
        $.each($InnerDivs, function (index, item) {
            var hndID = $($InnerDivs[index]).find('input.hndserviceID').val();
            var ddlValue = $($InnerDivs[index]).find('select').val();
            var inputvalue = $($InnerDivs[index]).find('input.serviceinput').val();
            var ServiceID = $($InnerDivs[index]).find('input.serviceinput').attr('id')

            var UsersServiceModel = new Object();
            UsersServiceModel.ID = hndID;
            UsersServiceModel.ServiceID = ServiceID;
            UsersServiceModel.RatesUnit = ddlValue;
            UsersServiceModel.ServiceRates = inputvalue;

            ServiceRatesList.push(UsersServiceModel)

        })
        var UserModel = {};
        $.each(model, function () {
            if (UserModel[this.name] !== undefined) {
                if (!UserModel[this.name].push) {
                    UserModel[this.name] = [UserModel[this.name]];
                }
                UserModel[this.name].push(this.value || '');
            } else {
                UserModel[this.name] = this.value || '';
            }
        });

        var VehicleModel = {};
        $.each(vehicle, function () {
            if (VehicleModel[this.name] !== undefined) {
                if (!VehicleModel[this.name].push) {
                    VehicleModel[this.name] = [VehicleModel[this.name]];
                }
                VehicleModel[this.name].push(this.value || '');
            } else {
                VehicleModel[this.name] = this.value || '';
            }
        });

        var UserViewModel = {
            "VehicleModelData": VehicleModel,
            "UserModel": UserModel,
            "objroleList": objroleList,
            "objFormTypeList": objFormTypeList,
            "ServiceRatesList": ServiceRatesList,
            "profilepicfolder": profilepicfolder,
            "vehiclepicfolder": vehiclepicfolder
        }

        var installermodel = $("#InstallerForm").serialize();
        $.ajax({
            url: uri,


            // data: (model ? model + "&" : "") + "objroleList=" + objroleList + "&ServiceRatesList" + ServiceRatesList,
            // data: { UserModel: UserModel, objroleList: objroleList, ServiceRatesList: ServiceRatesList },
            //  traditional: true,
            data: JSON.stringify(UserViewModel),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () {
                waitingDialog.show('Loading Please Wait...');
            },
            complete: function () {
                waitingDialog.hide();
            },
            type: 'POST',
            success: function (msg) {

                if (msg.success == true) {

                    //$("#myModal").modal('hide');
                    //bindActiveTable();
                    //$("#txtUserId").val('');
                    //$("#txtLogInId").val('');
                    //$("#txtUserFullName").val('');
                    //ClearUser();
                    var $toast = toastr["success"](msg.returnMessage, "Success Notification");

                    setTimeout(
      function () {
          location.href = $("#urlIndexpage").val();
      }, 5000);



                } else {
                    var $toast = toastr["error"](msg.returnMessage, "Error Notification");
                    //var $toast = toastr["error"]("Login Id is already taken", "Error Notification");
                }
                spinner.stop(target);


            },
            error: function (error) {
                console.log(error)
                //alert("something seems wrong");
            }
        });
    }

    $.extend($.fn.dataTable.defaults, {
        "searching": false,
        "ordering": false
    });

    $("#btnSave").click(function () {

        


        var objroleList = $('#ulRole li.highlighted').map(function (i, el) { return $(el)[0].id; });



        var Gender = $('#ddlGender').val()

        // $("#txtTerritoryName").focus();
        var Roles = $('#ddlPrimaryRole').val();



        if ($('#form3').valid()) {




            if (objroleList != null && objroleList.length > 0) {


                if ($('#form3').valid() && $("#txtPrimaryRoleId option:selected").text() != "--Select--" || $("input:radio[name='UserName1']").is(":checked")) {
                    //ManagerApprovalRequired


                    var state = $('#ddlState').val()
                    var City = $('#ddlCity').val()
                    var formType = $('#ulFormType li.highlighted').map(function (i, el) {
                        return $(el).val();
                    });
                    if (City == "" && $('#form3').valid()) {
                        var $toast = toastr["error"]('Please select City', 'Error Notification');
                        return;
                    }
                    else if (formType.length == 0 && $('#form3').valid()) {
                        var $toast = toastr["error"]('Please select formType', 'Error Notification');
                        return;
                    }
                    else if (state == "" && $('#form3').valid()) {
                        var $toast = toastr["error"]('Please select state', 'Error Notification');
                        return;
                    }

                    var roleNames = [];

                    $('#ulRole li.highlighted').map(function (i, el) { roleNames.push($("#" + $(el)[0].id).text()); });

                    var ManagerId = $('#ManagerId').val();
                    var FieldSupervisorId = $('#FieldSupervisorId').val();

                    if (roleNames != undefined && roleNames != null && roleNames.length > 0 && (roleNames.indexOf("Field Supervisor") > -1 || roleNames.indexOf("Manager") > -1)) {


                        if (roleNames.indexOf("Manager") > -1) {

                        }
                        else if (roleNames.indexOf("Field Supervisor") > -1 && (ManagerId === null || ManagerId === "")) {


                            var $toast = toastr["error"]('Please select manager for the user.', 'Error Notification');
                            return;
                        }
                        else if (roleNames.indexOf("Field Supervisor") > -1 && (ManagerId === null || ManagerId === "") && ($('#ManagerApprovalRequired').val() === "" || $('#ManagerApprovalRequired').val() === "0") && $('#form3').valid()) {
                            var $toast = toastr["error"]('Please select transfer approval status', 'Error Notification');
                            return;
                        }


                    }
                    else if (roleNames != undefined && roleNames != null && roleNames.length > 0 && roleNames.indexOf("Installer") > -1) {

                        if ((ManagerId === null || ManagerId === "")) {
                            var $toast = toastr["error"]('Please select manager for the user.', 'Error Notification');
                            return;
                        }
                        //else if (FieldSupervisorId === null || FieldSupervisorId === "") {
                        //    var $toast = toastr["error"]('Please select field supervisor for the user.', 'Error Notification');
                        //    return;
                        //}
                        else if (($('#ManagerApprovalRequired').val() === "" || $('#ManagerApprovalRequired').val() === "0") && $('#form3').valid()) {
                            var $toast = toastr["error"]('Please select transfer approval status', 'Error Notification');
                            return;
                        }
                    }
                    else if (roleNames != undefined && roleNames != null && roleNames.length > 0 && roleNames.indexOf("Auditor") > -1) {

                        if (ManagerId === null || ManagerId === "") {
                            var $toast = toastr["error"]('Please select manager for the user.', 'Error Notification');
                            return;
                        }
                        //else if (FieldSupervisorId === null || FieldSupervisorId === "") {
                        //    var $toast = toastr["error"]('Please select field supervisor for the user.', 'Error Notification');
                        //    return;
                        //}
                        //else if (($('#ManagerApprovalRequired').val() === "" || $('#ManagerApprovalRequired').val() === "0") && $('#form3').valid()) {
                        //    var $toast = toastr["error"]('Please select transfer approval status', 'Error Notification');
                        //    return;
                        //}
                    }













                    EditRecord();
                    GolbalRowID = 0;
                    $("#lblUsername").text("");
                    $("#lblRole").text("");
                    if ($("input:radio[name='UserName']").is(":checked")) {

                    }
                    $("#lblUsername").text("Please select user name type");
                    if ($("#ddlRole option:selected").text() != "--Select--") {
                        $("#lblRole").text("Role Required");
                        $("#lblRole").focus();
                        return false
                    }


                    if ($("#txtLogInId").val() == "") {
                        $("#txtLogInId").focus();
                        return false;
                    }
                    if ($("#txtEmail").val() == "") {
                        $("#txtEmail").focus();
                        return false;
                    }
                    if ($("#txtPassword").val() == "") {
                        $("#txtPassword").focus();
                        return false;
                    }
                    if ($("#txtMobileNo").val() == "") {
                        $("#txtMobileNo").focus();
                        return false;
                    }
                    if ($("#txtConfirmPassword").val() == "") {
                        $("#txtConfirmPassword").focus();
                        return false;
                    }
                    if ($("#txtUserAddress").val() == "") {
                        $("#txtUserAddress").focus();
                        return false;
                    }
                    if ($("#txtUserFullName").val() == "") {
                        $("#txtUserFullName").focus();
                        return false;
                    }

                    if ($("#ddlPrimaryRole option:selected").text() == "--Select--") {
                        $("#ddlPrimaryRole").focus();
                        return false;
                    }


                    if ($("#ManagerApprovalRequired").val() === "0") {
                        $("#ManagerApprovalRequired").focus();
                        return false;
                    }

                    //ClearUser();
                    //var $toast = toastr["success"](msg.returnMessage, "Success Notification");
                }
            }
            else {
                var $toast = toastr["error"]('Please select Role', 'Error Notification');
                return false;
            }

        }
    })



    function checkRoleStatus() {
        var Roleselect = $('#ddlPrimaryRole').val();
        if (Roleselect == null || Roleselect.length == 0) {
            var $toast = toastr["error"]('Please select Role', 'Error Notification');
        }
    }




    var BindRoles
    function Edit(UserID) {
        


        var urlGetData = $('#urlGetData').val();

        $.ajax({
            type: 'GET',
            url: urlGetData,
            beforeSend: function () {
                waitingDialog.show('Loading Please Wait...');
            },
            complete: function () {
                waitingDialog.hide();
            },
            data: { id: UserID },
            success: function (msg) {
                

                // RemoveErrorClass();
                TmpListRemove(0);
                GetState()
                BindCity(msg.modelData.StateID)
                GetRole();
                GetVehicleMakeList()
                $("#txtUserId").val(msg.modelData.UserId);
                $("#txtLogInId").val(msg.modelData.UserName);
                $("#txtEmail").val(msg.modelData.Email);
                $("#txtMobileNo").val(msg.modelData.MobileNo);
                $("#txtUserAddress").val(msg.modelData.Address);
                //$("#txtUserFullName").val(msg.modelData.FirstName);
                $("#txtUserFullName").val(msg.modelData.FirstName);
                $("#txtPassword").val(msg.modelData.Password);
                $("#txtConfirmPassword").val(msg.modelData.Password);

                //new added 
                $('#ddlState').find('option[value=' + msg.modelData.StateID + ']').attr('selected', 'selected');

                $('#ddlCity').find('option[value=' + msg.modelData.CityID + ']').attr('selected', 'selected');

                $('#ddlGender').find('option[value=' + msg.modelData.Gender + ']').attr('selected', 'selected');
                $('#ddlGender').trigger('chosen:updated');

                //Image             
                ChangePreviewLogo(msg.modelData.InstallerImage);
                ChangevehiclePreviewLogo(msg.modelData.ImageofVehicle)

                BindUserFormTypeByUserID(msg.modelData.UserId)

                $("#txtZip").val(msg.modelData.ZipCode);


                $("#txtHomePhoneNumber").val(msg.modelData.HomePhoneNumber);
                if (msg.modelData.VehicleMake != null && msg.modelData.VehicleMake != "") {
                    $('#ddlVehicleMake').find('option[value=' + msg.modelData.VehicleMake + ']').attr('selected', 'selected');
                    $('#ddlVehicleMake').trigger("change");
                }
                if (msg.modelData.VehicleModel != null && msg.modelData.VehicleMake != "") {
                    $('#ddlVehicleModel').find('option[value=' + msg.modelData.VehicleModel + ']').attr('selected', 'selected');
                }

                $("#txtYear").val(msg.modelData.VehicleYear);
                $("#txtLicensePlateNumber").val(msg.modelData.VehicleLicensePlateNumber);
                $("#txtVehicle").val(msg.modelData.Vehicle);
                $("#txtColor").val(msg.modelData.VehicleColor);
                // $("#txtImageofVehicle").val(msg.modelData.ImageofVehicle);
                $("#txtJobType").val(msg.modelData.JobType);

                if (msg.modelData.ManagerId != undefined) {

                    $('#ManagerId').find('option[value=' + msg.modelData.ManagerId + ']').attr('selected', 'selected').trigger('chosen:updated');
                }
                SetFieldSupervisorDropdown();
                if (msg.modelData.FieldSupervisorId != undefined) {
                                       
                    $('#FieldSupervisorId').find('option[value=' + msg.modelData.FieldSupervisorId + ']').attr('selected', 'selected').trigger('chosen:updated');
                }


                if (msg.modelData.ManagerApprovalRequired != undefined) {
                    $('#ManagerApprovalRequired').val(msg.modelData.ManagerApprovalRequired)
                }
                else {
                    $('#ManagerApprovalRequired').val(2);
                }



                $('.chosen-select').trigger('chosen:updated');
                if (msg.ServiceRatesList.length > 0) {
                    var count = 0;
                    var $div;
                    $("#servicesDiv").empty();

                    $.each(msg.ServiceRatesList, function (index, item) {

                        if (count == 0) {
                            //$div = $("<div>", { class: "form-group col-lg-12 col-sm-12 col-xs-12 usr_mob" });

                            $div = $("<div>", { class: "form-group " });

                            var $ServiceID = $("<input>", { name: item.ID, type: "hidden", value: item.ID, class: "hndserviceID" });

                            var $label = $("<label>", {
                                text: item.ServiceName, class: "col-lg-2 col-sm-1 control-label-1 ", style: "margin-left: -7%;margin-right:2%;margin-bottom:3%;width:42%;white-space:normal; "
                            });

                            var $input = $("<input>", { id: item.ServiceID, value: item.ServiceRates, type: "text", maxlength: "50", style: "text-align:right", class: "col-lg-4 col-sm-5 Decimal-Only serviceinput" });

                            var $select = $("<select>", { id: "", class: "" });
                            if (item.RatesUnit == "%") {
                                var $option1 = $("<option>", { text: "$", value: "$", class: "" });
                                var $option2 = $("<option>", { text: "%", selected: "selected", value: "%", class: "" });
                            } else {
                                var $option1 = $("<option>", {
                                    text: "$", selected: "selected", value: "$", class: ""
                                });
                                var $option2 = $("<option>", {
                                    text: "%", value: "%", class: ""
                                });
                            }

                            var $innerdiv1 = $("<div>", { id: "", class: "col-lg-1 col-sm-0" });
                            var $innerdiv = $("<div>", { id: "", class: "col-lg-5 col-sm-6 serviceInnerDiv" });

                            $($select).append($option1).append($option2);
                            $($innerdiv).append($ServiceID);
                            $($innerdiv).append($label);


                            $($innerdiv).append($input);
                            $($innerdiv).append($select);

                            $($div).append($innerdiv1);
                            $($div).append($innerdiv);

                            if (index === msg.ServiceRatesList.length - 1) {

                                $("#servicesDiv").append($div);
                            }

                            count++;

                            return true;
                        }
                        if (count == 1) {
                            var $ServiceID1 = $("<input>", { name: item.ID, type: "hidden", value: item.ID, class: "hndserviceID" });

                            var $label1 = $("<label>", {
                                text: item.ServiceName, class: "col-lg-2 col-sm-1 control-label-1 ", style: "margin-left: -7%;margin-right: 2%;margin-bottom:3%;width:42%;white-space:normal;"
                            });

                            var $input1 = $("<input>", { id: item.ServiceID, type: "text", value: item.ServiceRates, maxlength: "50", style: "text-align:right", class: "col-lg-4 col-sm-6 Decimal-Only serviceinput" });

                            var $select1 = $("<select>", { id: "", class: "" });

                            if (item.RatesUnit == "%") {
                                var $option11 = $("<option>", { text: "$", value: "$", class: "" });
                                var $option12 = $("<option>", { text: "%", selected: "selected", value: "%", class: "" });
                            } else {
                                var $option11 = $("<option>", {
                                    text: "$", selected: "selected", value: "$", class: ""
                                });
                                var $option12 = $("<option>", {
                                    text: "%", value: "%", class: ""
                                });
                            }
                            var $innerdiv2 = $("<div>", { id: "", class: "col-lg-1 col-sm-0" });
                            var $innerdiv1 = $("<div>", { id: "", class: "col-lg-5 col-sm-6 serviceInnerDiv" });

                            $($select1).append($option11).append($option12);


                            $($innerdiv1).append($ServiceID1);
                            $($innerdiv1).append($label1);
                            $($innerdiv1).append($input1);
                            $($innerdiv1).append($select1);


                            //$($div).append($innerdiv);
                            $($div).append($innerdiv1);
                            $($div).append($innerdiv2);
                            $("#servicesDiv").append($div);
                            count = 0;

                        }



                    })

                    //  $(".chosen-select").chosen({
                    //disable_search: true, no_results_text: "Oops, nothing found!", width: 155
                    //});
                }




                if (msg.ModelCD.length > 0) {

                    $.each(msg.ModelCD, function (index, item) {

                        $("#ddlPrimaryRole").multiselect().find(":checkbox[value='" + item.Id + "']").attr("checked", "checked")
                        $("#ddlPrimaryRole option[value='" + item.Id + "']").attr("selected", 1);



                        $("#ulRole").find("li[id='" + item.Id + "']").addClass("ui-selected click-selected highlighted")

                    });

                    selectRole()



                    //
                    var objroleList = []

                    $('#ulRole li.highlighted').map(function (i, el) { objroleList.push($("#" + $(el)[0].id).text()); });




                    if (objroleList != undefined && objroleList.length > 0 && (objroleList.indexOf("Field Supervisor") > -1 || objroleList.indexOf("Manager") > -1)) {

                        if (objroleList.indexOf("Field Supervisor") > -1) {
                            $('#ManagerId').prop('disabled', false).trigger("chosen:updated");
                            $('#FieldSupervisorId').val('0').attr("selected", "selected").prop('disabled', true).trigger("chosen:updated");

                        }

                        if (objroleList.indexOf("Manager") > -1) {
                            $('#ManagerId').val('0').attr("selected", "selected").prop('disabled', true).trigger("chosen:updated");
                            $('#FieldSupervisorId').val('0').attr("selected", "selected").prop('disabled', true).trigger("chosen:updated");
                            $("#ManagerApprovalRequired").val('2').attr("selected", "selected").prop('disabled', true).trigger("chosen:updated");
                        }
                    }
                    else if (objroleList != undefined && objroleList.length > 0 && (objroleList.indexOf("Installer") > -1 || objroleList.indexOf("Auditor") > -1)) {

                        $('#ManagerApprovalRequired').prop('disabled', false).trigger("chosen:updated");
                        $('#ManagerId').prop('disabled', false).trigger("chosen:updated");
                      
                        $('#FieldSupervisorId').prop('disabled', false).trigger("chosen:updated");

                    }
                    else {

                        $('#ManagerId').val('0').attr("selected", "selected").prop('disabled', true).trigger("chosen:updated");
                        $('#FieldSupervisorId').val('0').attr("selected", "selected").prop('disabled', true).trigger("chosen:updated");
                    }





                    // checkinstallerroleselect()     Commented by sominath
                    $('.InstallerRoleDiv').show();    // Modify by sominath

                    CheckAssignedProjects(UserID, msg.modelData.ManagerId)
                    // $("#ddlPrimaryRole").multiselect('rebuild');
                }
                else {
                    // GetRole();
                    // $("#ddlPrimaryRole").multiselect("refresh");

                }

               

                count = msg.TCount + 1;
                TmpListRemove(0);


                var PrimaryRole = $('#ddlPrimaryRole option:selected');


            },


            error: function (error) {
                console.log(error)
                //alert("something seems wrong");
            }

        });


        // spinner.stop(target);

        //$('#ddlCountry').val(BindCon).attr("selected", "selected");
    }


    function EditUserDetails(id) {

        var urlGetData = $('#EditContactDetalisUrl').val();

        $.ajax({
            type: 'GET',
            url: urlGetData,
            // contentType: "application/json; charset=utf-8",
            // dataType: "json",
            data: { id: id },
            success: function (msg) {

                // $("#txtID").val(msg.modelData.TerritoryID);

                $("#txtFirstName").val(msg.FirstName);
                $("#txtLastName").val(msg.LastName);
                $("#txtMobileNumber").val(msg.MobileNumber);
                $("#txtUserAddress").val(msg.Address);
                $("#txtEmailId").val(msg.EmailId);
                EditMobile = msg.TempID;
                spinner.stop(target);

            },
            error: function (error) {
                console.log(error)
                //alert("something seems wrong");
            }

        });
    }


    function GetState() {

        var urlStateData = $('#StateListUrl').val();

        $.ajax({
            type: 'POST',
            url: urlStateData,
            async: false,
            success: function (data) {


                $('#ddlState').empty();

                StateData = data;

                $.each(data, function (index, item) {
                    if (index == 0) {
                        $('#ddlState').append(
                          $('<option></option>').val("").html(""));
                    }

                    $('#ddlState').append(
                           $('<option></option>').val(item.StateID).html(item.StateName)
                     );
                });

                $('.chosen-select').trigger('chosen:updated');
            },
            error: function (error) {
                console.log(error)
                //alert("something seems wrong");
            }

        });

    }


    $('#ddlState').change(function () {

        var stateid = $('#ddlState option:selected').val();
        if (stateid != undefined && stateid != "0") {

            BindCity(stateid)

        }

    })

    function BindCity(stateid) {
        var urlCityData = $('#CityByStateIDListUrl').val();
        $.ajax({
            type: 'GET',
            data: {
                "stateId": stateid,
                "cityName": ''
            },
            async: false,
            url: urlCityData,
            success: function (data) {

                $('#ddlCity').empty();
                $.each(data, function (index, item) {
                    if (index == 0) {
                        $('#ddlCity').append(
                          $('<option></option>').val('').html(""));
                    }

                    $('#ddlCity').append(
                           $('<option></option>').val(item.CityID).html(item.CityName)
                     );
                });

                $('.chosen-select').trigger('chosen:updated');
            },

        });
    }


    function bindServices() {

        var urlActiveRecords = $("#ServicesListUrl").val();
        $.ajax({
            url: urlActiveRecords,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            //  data: JSON.stringify(WidgetViewModel),
            type: 'POST',
            success: function (msg) {

                var count = 0;
                var $div;
                $("#servicesDiv").empty();
                $.each(msg, function (index, item) {

                    if (count == 0) {
                        $div = $("<div>", { id: "", class: "form-group " });

                        var $ServiceID = $("<input>", { name: item.ID, type: "hidden", value: "", class: "hndserviceID" });

                        var $label = $("<label>", { text: item.Service, class: "col-lg-2 col-sm-1 control-label-1 ", style: "margin-left: -7%;margin-right: 2%;margin-bottom:3%;width:42%;white-space:normal; " });

                        var $input = $("<input>", { id: item.ID, lang: item.ServiceBasePrice, type: "text", maxlength: "50", style: "text-align:right", class: "col-lg-4 col-sm-5 Decimal-Only serviceinput" });

                        var $select = $("<select>", { id: "", class: "" });

                        var $option1 = $("<option>", { text: "$", selected: "selected", value: "$", class: "" });
                        var $option2 = $("<option>", { text: "%", value: "%", class: "" });

                        //var $innerdiv = $("<div>", { id: "", class: "col-lg-6 col-sm-6 serviceInnerDiv" });
                        var $innerdiv1 = $("<div>", { id: "", class: "col-lg-1 col-sm-0" });
                        var $innerdiv = $("<div>", { id: "", class: "col-lg-5 col-sm-6 serviceInnerDiv" });

                        $($select).append($option1).append($option2);
                        $($innerdiv).append($ServiceID);
                        $($innerdiv).append($label);
                        $($innerdiv).append($input);
                        $($innerdiv).append($select);


                        $($div).append($innerdiv1);
                        $($div).append($innerdiv);


                        if (index === msg.length - 1) {

                            $("#servicesDiv").append($div);
                        }

                        count++;

                        return true;
                    }
                    if (count == 1) {
                        var $ServiceID1 = $("<input>", { name: item.ID, type: "hidden", value: "", class: "hndserviceID" });

                        var $label1 = $("<label>", { text: item.Service, class: "col-lg-1 col-sm-1 control-label-1 usrserv-2", style: "margin-left: -7%;margin-right: 2%;margin-bottom:3%;width:42%;white-space:normal;" });

                        var $input1 = $("<input>", { id: item.ID, lang: item.ServiceBasePrice, type: "text", maxlength: "50", style: "text-align:right", class: "col-lg-4 col-sm-5 Decimal-Only serviceinput" });

                        var $select1 = $("<select>", { id: "", class: "" });

                        var $option11 = $("<option>", { text: "$", value: "$", class: "" });
                        var $option12 = $("<option>", { text: "%", value: "%", class: "" });

                        var $innerdiv1 = $("<div>", { id: "", class: "col-lg-5 col-sm-6 serviceInnerDiv" });


                        $($select1).append($option11).append($option12);


                        $($innerdiv1).append($ServiceID1);
                        $($innerdiv1).append($label1);
                        $($innerdiv1).append($input1);
                        $($innerdiv1).append($select1);

                        //$($div).append($innerdiv);
                        $($div).append($innerdiv1);
                        $("#servicesDiv").append($div);
                        count = 0;

                    }



                })
                //$(".chosen-select").chosen({ disable_search: true, no_results_text: "Oops, nothing found!", width: 155 });
            }
        });


    }

    function CalculateServiceRates() {

        var $InnerDivs = document.getElementsByClassName('serviceInnerDiv');
        var ServiceRatesList = [];
        $.each($InnerDivs, function (index, item) {
            var hndID = $($InnerDivs[index]).find('input.hndserviceID').val();
            var ddlValue = $($InnerDivs[index]).find('select').val();
            var inputvalue = $($InnerDivs[index]).find('input.serviceinput').val();
            var ServiceID = $($InnerDivs[index]).find('input.serviceinput').attr('id')

            var ServiceRate = new Object();
            ServiceRate.ID = hndID;
            ServiceRate.ServiceID = ServiceID;
            ServiceRate.RatesUnit = ddlValue;
            ServiceRate.ServiceRates = inputvalue;

            ServiceRatesList.push(ServiceRate)

        })


    }



    // #region numeric only
    $('body').on('keypress', '.Numeric-Only', function (event) {

        var regex = new RegExp("^[0-9\b]+$");

        if (event.charCode == 0)
        { return true; }
        else {
            var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
            if (!regex.test(key)) {
                event.preventDefault();
                return false;
            }
        }
    });


    // #endregion

    // #region decimal only
    $('body').on('keypress', '.Decimal-Only', function (event) {
        return isNumber(event, this)
    });

    function isNumber(evt, element) {

        var charCode = (evt.which) ? evt.which : event.keyCode

        if (
            (charCode != 45 || $(element).val().indexOf('-') != -1) &&      // “-” CHECK MINUS, AND ONLY ONE.
            (charCode != 46 || $(element).val().indexOf('.') != -1) &&      // “.” CHECK DOT, AND ONLY ONE.
            (charCode < 48 || charCode > 57))
            return false;

        return true;
    }

    // #endregion


    function CheckAssignedProjects(UserID, selectedManagerId) {
        var urlCheckAssignedProjects = $('#urlCheckAssignedProjects').val();
        $.ajax({
            type: 'GET',
            url: urlCheckAssignedProjects,
            data: { id: UserID },
            success: function (msg) {
                $("#ProjectsassignedDiv").hide();
                var allProjectIds = [];
                try {
                    var projectIds = $.map(msg, function (x) { return x.project_Id; });
                    allProjectIds = projectIds;
                } catch (e) {
                    console.log(e);
                }

                // Ensure manager list is refreshed for this user;
                var managerToSelect = selectedManagerId;
                if (managerToSelect == undefined || managerToSelect == null || managerToSelect == "") {
                    managerToSelect = $('#ManagerId').val();
                }

                // Bind manager list for all projects assigned
                BindManagerList(allProjectIds, managerToSelect);

                if (msg.length > 0) {
                    var count = 0;
                    var $div;
                    $("#InstallerProjectDiv").empty();

                    $.each(msg, function (index, item) {
                        $("#ProjectsassignedDiv").show();
                        if (count == 0) {
                            $div = $("<tr>", {});

                            var $label = $("<span>", {
                                text: item.projectName, class: "", style: ""
                            });

                            var $innerdiv = $("<td>", {});
                            var $innerdivempty = $("<td>", {});

                            $($innerdiv).append($label);

                            $($div).append($innerdiv);

                            if (index === msg.length - 1) {
                                $($div).append($innerdivempty);
                                $("#InstallerProjectDiv").append($div);
                            }

                            count++;

                            return true;
                        }
                        if (count == 1) {

                            var $label1 = $("<span>", {
                                text: item.projectName, class: "", style: ""
                            });

                            var $innerdiv1 = $("<td>", {});

                            $($innerdiv1).append($label1);

                            $($div).append($innerdiv1);
                            $("#InstallerProjectDiv").append($div);
                            count = 0;

                        }
                    })

                    $(".chosen-select").chosen({
                        //  disable_search: true, no_results_text: "Oops, nothing found!", width: 155
                    });
                }

            },
            error: function (error) {
                console.log(error);
            }
        });
    }


    function GetFormTypeList() {

        var urlGetFormTypeList = $('#urlGetFormTypeList').val();
        $.ajax({
            type: 'GET',
            // data: {},
            async: false,
            url: urlGetFormTypeList,
            success: function (data) {

                $('#ulFormType').empty();
                $("#FormTypeHeader").removeClass("ddlHeader").text("Select Form Type")
                $("#FormType_SelectAll").prop("checked", false);
                $.each(data, function (index, item) {

                    $('#ulFormType').append($('<li></li>').val(item.ID).addClass("liFormType active-result").html(item.FormType));
                    //if (index == 0) {
                    //    $('#ddlFormType').append(
                    //      $('<option></option>').val('').html(""));
                    //}

                    //$('#ddlFormType').append(
                    //       $('<option></option>').val(item.ID).html(item.FormType)
                    // );
                });

                $('.chosen-select').trigger('chosen:updated');
            },

        });

    }



    //#region code for Logo Save in folder

    var profilepicfolder;
    function Filepath() {
        
        formData = "";
        waitingDialog.show('Loading Please Wait...');
        var urlLogoupload = $('#urlLogoupload').val();
        if ($("#fileLogoURL").val() != "") {

            var pathArray = $("#fileLogoURL").val();
            pathArray = pathArray.replace(/(c:\\)*fakepath\\/i, "");
            $('#txtLogoURL').val(pathArray);
            // PreviewImagePath = $('#txtLogoURL').val();
            if (pathArray != '') {
                var a1 = pathArray.split('.');
                var length = a1.length;
                if (a1[length - 1].toLowerCase() != 'jpg' && a1[length - 1].toLowerCase() != 'jpeg' && a1[length - 1].toLowerCase() != 'png') {
                    var $toast = toastr["error"]("Please Select File .jpg/.jpeg/.png Only", "Error Notification");
                    $('#fileLogoURL').val('');
                    return;
                }
                else {


                }
                //$('#profileimgremove').hide();
                $("#MainImgLOgo").attr('src', '');

                var formData = new FormData();
                var totalFiles = document.getElementById("fileLogoURL").files.length;
                for (var i = 0; i < totalFiles; i++) {
                    var file = document.getElementById("fileLogoURL").files[i];

                    formData.append("LogoURL", file);
                }

                var uID = $('#EditUserID').val();
                formData.append("UserId", uID);

                $.ajax({
                    type: "POST",
                    url: urlLogoupload,
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    success: function (response) {
                        
                        if (response.Msg == true) {
                            profilepicfolder = response.strfilepath;
                            // $('#profileimgremove').show();

                            // profilepicfolder = response;
                            var $toast = toastr["success"]("Image uploaded successfully", "Notification");

                        }
                        waitingDialog.hide();
                    },
                    error: function (error) {
                        var $toast = toastr["error"]("Error,in File Saving", "Notification");
                        waitingDialog.hide();
                    }
                });



            }
        }
        else {

            
            var PreImgPath = $('#WebURL').val() + profilepicfolder;
            // var PreImgPath = "https://localhost:6069/" + profilepicfolder;
            // $("#form3 .fileupload-preview").children("img").attr("src", PreImgPath)   
            $("#MainImgLOgo").attr('src', PreImgPath);
            waitingDialog.hide();
        }




    }
    //#endregion


    function ChangePreviewLogo(Imagepath) {
        
        if (Imagepath == null || Imagepath == undefined || Imagepath == "") {
            imagePath = "https://www.placehold.it/200x150/EFEFEF/AAAAAA&amp;text=no+image";
            $('#profileimgremove').hide();

        }
        else {
            imagePath = $('#WebURL').val() + Imagepath;
            //imagePath = "https://localhost:6069/" + Imagepath;
            $("#txtLogoURL").val(Imagepath);
            $('#profileimgremove').show();
        }

        $("#MainImgLOgo").attr('src', '');
        $("#MainImgLOgo").attr('src', imagePath);

    }

    //#region vehicle Logo Change
    function ChangevehiclePreviewLogo(Imagepath) {
        
        if (Imagepath == null || Imagepath == undefined || Imagepath == "") {
            imagePath = "https://www.placehold.it/200x150/EFEFEF/AAAAAA&amp;text=no+image";
            $("#VehicleMainImgLOgo").attr('src', '');
            $("#VehicleMainImgLOgo").attr('src', imagePath);
            $('#vehicleimgremove').hide();
        }
        else {
            imagePath = $('#WebURL').val() + Imagepath;

            //imagePath = "https://localhost:6069/" + Imagepath;
            $("#txtImageofVehicle").val(Imagepath);

            //imagePath = WebURL + 'Uploads/' + PreviewImagePath;
            $("#VehicleMainImgLOgo").attr('src', '');
            $("#VehicleMainImgLOgo").attr('src', imagePath);
            $('#vehicleimgremove').show();
        }



    }
    //#endregion

    //#region code for vehicle Logo Save in folder

    var vehiclepicfolder;
    function FilepathVehicle() {

        var urlLogoupload = $('#urlLogouploadVehicle').val();
        if ($("#fileLogoURLVehicle").val() != "") {

            var pathArray = $("#fileLogoURLVehicle").val();
            pathArray = pathArray.replace(/(c:\\)*fakepath\\/i, "");
            $('#txtImageofVehicle').val(pathArray);
            // PreviewImagePath = $('#txtLogoURL').val();
            if (pathArray != '') {
                var a1 = pathArray.split('.');
                var length = a1.length;
                if (a1[length - 1].toLowerCase() != 'jpg' && a1[length - 1].toLowerCase() != 'jpeg' && a1[length - 1].toLowerCase() != 'png') {
                    var $toast = toastr["error"]("Please Select File .jpg/.jpeg/.png Only", "Error Notification");
                    $('#fileLogoURLVehicle').val('');
                    return;
                }
                else {


                }
                // $('#vehicleimgremove').hide();
                $("#VehicleMainImgLOgo").attr('src', '');

                var formData = new FormData();
                var totalFiles = document.getElementById("fileLogoURLVehicle").files.length;
                for (var i = 0; i < totalFiles; i++) {
                    var file = document.getElementById("fileLogoURLVehicle").files[i];

                    formData.append("LogoURL", file);
                }
                var uID = $('#EditUserID').val();
                formData.append("UserId", uID);

                $.ajax({
                    type: "POST",
                    url: urlLogoupload,
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                    success: function (response) {
                        
                        if (response.Msg == true) {
                            vehiclepicfolder = response.strfilepath;
                            var $toast = toastr["success"]("Image uploaded successfully", "Success Notification");
                        }
                        //vehiclepicfolder = response;
                    },
                    error: function (error) {
                        var $toast = toastr["error"]("Error,in File Saving", "Error Notification");
                    }
                });



            }
        }
        else {

            
            var PreImgPath = $('#WebURL').val() + vehiclepicfolder;
            //var PreImgPath = "https://localhost:6069/" + vehiclepicfolder;
            // $("#form3 .fileupload-preview").children("img").attr("src", PreImgPath)   
            $("#VehicleMainImgLOgo").attr('src', PreImgPath);

        }

    }
    //#endregion




    //#region New code for Role multiselect dropdown
    function selectRole() {

        var selectedlivalue = $('#ulRole li.highlighted').map(function (i, el) {
            return $(el).val();
        });


        var selectedlilength = selectedlivalue.length;
        if (selectedlilength == 0) {
            $("#RoleHeader").removeClass("ddlHeader").text("Select Role")
        } else {
            $("#RoleHeader").addClass("ddlHeader").text(selectedlilength + " Role selected")
        }

        //#region SelectAll checked/unchecked
        if ($('#ulRole .liRole:visible').length > 0 && $('#ulRole .highlighted:visible').length == $('#ulRole .liRole:visible').length) {
            $('#Role_SelectAll').prop('checked', true);
        } else {
            $('#Role_SelectAll').prop('checked', false);
        }
        //#endregion

    }

    //#region open/close Cycle dropdown
    $("#ancRole_chosen").click(function () {
        $("#ddlRole_chosen").toggleClass("chosen-with-drop chosen-container-active")
    });

    $('body').on('click', function (e) {
        if (!$('#divRole').is(e.target)
           && $('#divRole').has(e.target).length === 0
       ) {
            $('#ddlRole_chosen').removeClass('chosen-with-drop chosen-container-active');
        }
    });

    //#endregion

    function filterCycle(element) {

        var value = $(element).val().toLowerCase();

        $("#ulRole > li").each(function () {
            if ($(this).text().toLowerCase().search(value) > -1) {
                $(this).show();
            }
            else {
                $(this).hide();
            }
        });

        //#region SelectAll checked/unchecked
        if ($('#ulRole .liRole:visible').length > 0 && $('#ulRole .highlighted:visible').length == $('#ulRole .liRole:visible').length) {
            $('#Role_SelectAll').prop('checked', true);
        } else {
            $('#Role_SelectAll').prop('checked', false);
        }
        //#endregion
    }

    $('#Role_SelectAll').on('click', function () {
        if ($('#ulRole .liRole').length > 0) {




            if (this.checked) {
                $("li.liRole:visible").addClass('ui-selected click-selected highlighted');

                var selectedlitext = $('#ulRole .highlighted').map(function (i, el) {

                    return $(el)[0].value;

                });
                var selectedlilength = selectedlitext.length;
                if (selectedlilength == 0) {
                    $("#RoleHeader").removeClass("ddlHeader").text("Select Role")
                } else {
                    $("#RoleHeader").addClass("ddlHeader").text(selectedlilength + " Role selected")
                }

            }
            else {
                $("li.liRole:visible").removeClass('ui-selected click-selected highlighted');

                var selectedlitext = $('#ulRole .highlighted').map(function (i, el) {

                    return $(el)[0].value;

                });
                var selectedlilength = selectedlitext.length;
                if (selectedlilength == 0) {
                    $("#RoleHeader").removeClass("ddlHeader").text("Select Role")
                } else {
                    $("#RoleHeader").addClass("ddlHeader").text(selectedlilength + " Role selected")
                }
            }

            checkinstallerroleselect()
        }
    });

    //#endregion


    //#region New code for FormType multiselect dropdown
    function selectFormType() {

        var selectedlivalue = $('#ulFormType li.highlighted').map(function (i, el) {
            return $(el).val();
        });


        var selectedlilength = selectedlivalue.length;
        if (selectedlilength == 0) {
            $("#FormTypeHeader").removeClass("ddlHeader").text("Select Form Type")
        } else {
            $("#FormTypeHeader").addClass("ddlHeader").text(selectedlilength + " Form Type selected")
        }

        //#region SelectAll checked/unchecked
        if ($('#ulFormType .liFormType:visible').length > 0 && $('#ulFormType .highlighted:visible').length == $('#ulFormType .liFormType:visible').length) {
            $('#FormType_SelectAll').prop('checked', true);
        } else {
            $('#FormType_SelectAll').prop('checked', false);
        }
        //#endregion

    }

    //#region open/close Cycle dropdown
    $("#ancFormType_chosen").click(function () {
        $("#ddlFormType_chosen").toggleClass("chosen-with-drop chosen-container-active")
    });

    $('body').on('click', function (e) {
        if (!$('#divFormType').is(e.target)
           && $('#divFormType').has(e.target).length === 0
       ) {
            $('#ddlFormType_chosen').removeClass('chosen-with-drop chosen-container-active');
        }
    });

    //#endregion

    function filterFormType(element) {

        var value = $(element).val().toLowerCase();

        $("#ulFormType > li").each(function () {
            if ($(this).text().toLowerCase().search(value) > -1) {
                $(this).show();
            }
            else {
                $(this).hide();
            }
        });

        //#region SelectAll checked/unchecked
        if ($('#ulFormType .liFormType:visible').length > 0 && $('#ulFormType .highlighted:visible').length == $('#ulFormType .liFormType:visible').length) {
            $('#FormType_SelectAll').prop('checked', true);
        } else {
            $('#FormType_SelectAll').prop('checked', false);
        }
        //#endregion
    }

    $('#FormType_SelectAll').on('click', function () {
        if ($('#ulFormType .liFormType').length > 0) {




            if (this.checked) {
                $("li.liFormType:visible").addClass('ui-selected click-selected highlighted');

                var selectedlitext = $('#ulFormType .highlighted').map(function (i, el) {

                    return $(el)[0].value;

                });
                var selectedlilength = selectedlitext.length;
                if (selectedlilength == 0) {
                    $("#FormTypeHeader").removeClass("ddlHeader").text("Select Form Type")
                } else {
                    $("#FormTypeHeader").addClass("ddlHeader").text(selectedlilength + " Form Type selected")
                }

            }
            else {
                $("li.liFormType:visible").removeClass('ui-selected click-selected highlighted');

                var selectedlitext = $('#ulFormType .highlighted').map(function (i, el) {

                    return $(el)[0].value;

                });
                var selectedlilength = selectedlitext.length;
                if (selectedlilength == 0) {
                    $("#FormTypeHeader").removeClass("ddlHeader").text("Select Form Type")
                } else {
                    $("#FormTypeHeader").addClass("ddlHeader").text(selectedlilength + " Form Type selected")
                }
            }

            // checkinstallerroleselect()
        }
    });

    //#endregion


    function checkinstallerroleselect() {
        debugger

        $('.InstallerRoleDiv').show();
        ClearInstaller()


    }



    function GetVehicleMakeList() {

        var urlGetVehicleMakeList = $('#urlGetVehicleMakeList').val();
        $.ajax({
            type: 'GET',
            // data: {},
            async: false,
            url: urlGetVehicleMakeList,
            success: function (data) {

                $('#ddlVehicleMake').empty();
                $('#ddlVehicleModel').empty();
                $.each(data, function (index, item) {
                    if (index == 0) {
                        $('#ddlVehicleMake').append(
                          $('<option></option>').val('').html(""));
                    }

                    $('#ddlVehicleMake').append(
                           $('<option></option>').val(item.MakeId).html(item.Make)
                     );
                });

                $('#ddlVehicleMake').trigger('chosen:updated');
            },

        });

    }

    $('#ddlVehicleMake').change(function () {
        var VehicleMakeID = $('#ddlVehicleMake').val();
        if (VehicleMakeID != null && VehicleMakeID != "" && VehicleMakeID != undefined) {
            GetVehicleModelList(VehicleMakeID)
        }
    })

    function GetVehicleModelList(VehicleMakeID) {

        var urlGetVehicleModelList = $('#urlGetVehicleModelList').val();
        $.ajax({
            type: 'GET',
            data: { VehicleMakeID: VehicleMakeID },
            async: false,
            url: urlGetVehicleModelList,
            success: function (data) {

                $('#ddlVehicleModel').empty();
                $.each(data, function (index, item) {
                    if (index == 0) {
                        $('#ddlVehicleModel').append(
                          $('<option></option>').val('').html(""));
                    }

                    $('#ddlVehicleModel').append(
                           $('<option></option>').val(item.ID).html(item.Model)
                     );
                });

                $('#ddlVehicleModel').trigger('chosen:updated');
            },

        });

    }

    function BindUserFormTypeByUserID(id) {
        debugger;

        var urlBindUserFormTypeByUserID = $('#urlBindUserFormTypeByUserID').val();
        $.ajax({
            type: 'GET',
            data: { UserID: id },
            async: false,
            url: urlBindUserFormTypeByUserID,
            success: function (data) {

                $.each(data, function (index, item) {

                    $("#ulFormType").find("li[value='" + item.FormTypeID + "']").addClass("ui-selected click-selected highlighted")

                });
                debugger;
                selectFormType()
            },

        });


    }


    $('#profileimgremove').click(function () {
        
        // $("#MainImgLOgo").attr('src', '');
        imagePath = "https://www.placehold.it/200x150/EFEFEF/AAAAAA&amp;text=no+image";

        // $("#MainImgLOgo").attr('src', imagePath);
        //  $("#txtLogoURL").val("");

        var uID = $('#EditUserID').val();
        // alert(uID);        
        //var path = $("#txtLogoURL").val();
        //alert(path);
        var urlRemoveProfileImg = $("#urlRemoveProfileImg").val();
        $.ajax({
            type: "GET",
            url: urlRemoveProfileImg,
            data: { userid: uID },
            async: false,
            success: function (data) {
                
                if (data.msg == true) {


                    $("#MainImgLOgo").attr('src', '');
                    $("#MainImgLOgo").attr('src', imagePath);
                    $("#form3 .fileupload-preview").children("img").attr("src", imagePath)
                    $("#txtLogoURL").val("");
                    $('#profileimgremove').hide();
                    //$("#btnChange").hide();
                    //$("#btnSeletImg").show();
                    profilepicfolder = '';
                    var $toast = toastr["success"](data.returnMsg, "Success Notification");
                }

            },
            error: function (error) {
                var $toast = toastr["error"]("Error,in File removing", "Error Notification");
            }
        });


        //$('#profileimgremove').hide();
        //profilepicfolder = '';





    })

    $('#vehicleimgremove').click(function () {
        
        $("#VehicleMainImgLOgo").attr('src', '');
        imagePath = "https://www.placehold.it/200x150/EFEFEF/AAAAAA&amp;text=no+image";


        var uID = $('#EditUserID').val();
        // alert(uID);
        // var path = $("#txtLogoURL").val();
        // alert(path);
        var urlRemoveVehicleImage = $("#urlRemoveVehicleImage").val();
        $.ajax({
            type: "GET",
            url: urlRemoveVehicleImage,
            data: { userid: uID },
            async: false,
            success: function (data) {
                
                if (data.msg == true) {
                    $("#VehicleMainImgLOgo").attr('src', imagePath);
                    $("#txtImageofVehicle").val("");
                    $('#vehicleimgremove').hide();
                    vehiclepicfolder = '';
                    var $toast = toastr["success"](data.returnMsg, "Success Notification");
                }

            },
            error: function (error) {
                var $toast = toastr["error"]("Error,in File removing", "Error Notification");
            }
        });

        //$("#VehicleMainImgLOgo").attr('src', imagePath);
        //$("#txtImageofVehicle").val("");
        //$('#vehicleimgremove').hide();


    })
    $(document).ready(function () {
        $('#showPassword').on('hover', function () {
            var passwordInput = $('#txtPassword').attr('type');;

            if (passwordInput === 'password') {
                $('#txtPassword').attr('type', 'text');
            } else {
                $('#txtPassword').attr('type', 'password');
            }
        });

        $('#showConfirmPassword').on('hover', function () {
            var passwordInput = $('#txtConfirmPassword').attr('type');

            if (passwordInput === 'password') {
                $('#txtConfirmPassword').attr('type', 'text');
            } else {
                $('#txtConfirmPassword').attr('type', 'password');
            }
        });

        //$('#showPassword').hide();
        // $('#showConfirmPassword').hide();
    });
    $("#ManagerId").change(function () {

        SetFieldSupervisorDropdown();
    })

    function SetFieldSupervisorDropdown()
    {
        var urlManager = $('#urlGetSuperVisorList').val();

        var ManagerId = $('#ManagerId').val();
        $('#FieldSupervisorId').empty();
        if (ManagerId != undefined && ManagerId != "" && ManagerId != "0") {
            $.ajax({

                type: 'POST',
                async: false,
                data: { "ManagerId": ManagerId },

                url: urlManager,



                success: function (data) {

                    

                    $('#FieldSupervisorId').empty();

                    $.each(data, function (key, value) {

                        if (key == 0) {

                            $('#FieldSupervisorId').append($('<option></option>').val("").html(""));
                        }
                        $('#FieldSupervisorId').append($('<option></option>').val(value.ChildId).html(value.ChildName));

                    });

                    $('#FieldSupervisorId').trigger("chosen:updated");
                },
                error: function (error) {
                    console.log(error)
                }

            });
        }
    }


}
catch (e) {
    console.log(e)
    alert(e.name + ': ' + e.message);
}