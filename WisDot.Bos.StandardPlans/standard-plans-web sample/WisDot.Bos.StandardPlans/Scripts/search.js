$(document).ready(function () {
    var validFiipsConstructionId = false;
    var validStructureId = true;
    var fiipsConstructionId = "";
    var fiipsDesignId = "";
    var fiipsStructureId = "";
    var agreementUrl = "";
    var clicked = false;

    $("#acceptImageDiv").hide();
    $("#rejectImageDiv").hide();
    $("#ImageDiv1").hide();
    $("#ImageDiv2").hide();
    $("#acceptImageDivS").hide();
    $("#rejectImageDivS").hide();
    $("#ImageDivSA").hide();
    $("#ImageDivSR").hide();
    $("#ajaxLoaderDiv").hide();
    $("#getPlan").prop("disabled", true);
    $("#projectTypeStateLet").prop("checked", true);
    //$("#projectTypeLocalLet").prop("disabled", true);
    enableLocalLet();

    $("#localLetRequirements").hide();

    $("#projectTypeStateLet").click(function () {
        $("#localLetAgreeToTermsOfUse").prop("checked", false);
        $("#stateLetRequirements").show();
        $("#localLetRequirements").hide();
        toggleGetPlan();
    });

    $("#projectTypeLocalLet").click(function () {
        $("#stateLetAgreeToTermsOfUse").prop("checked", false);
        $("#stateLetRequirements").hide();
        $("#localLetRequirements").show();
        fiipsConstructionId = "";
        fiipsDesignId = "";
        fiipsStructureId = "";
        toggleGetPlan();
    });

    $("#getPlan").click(function (event) {
            var spanLength = $("#spanLength").val();
            var substructureSkew = $("#substructureSkew").val();
            var clearRoadwayWidth = $("#clearRoadwayWidth").val();
            var barrierType = $("#barrierType").val();
            var pavingNotch = $("#pavingNotch").prop("checked");
            var abutmentHeight = $("#abutmentHeight").val();
            var pilingType = $("#pilingType").val();
            $.ajax({
                type: "POST",
                url: urlSettings.getPlan,
                data: '{spanLength: "' + spanLength + '", substructureSkew: "' + substructureSkew + '", clearRoadwayWidth: "' + clearRoadwayWidth + '", barrierType: "' + barrierType + '", pavingNotch: "' + pavingNotch + '", abutmentHeight: "' + abutmentHeight + '", pilingType: "' + pilingType + '", fiipsConstructionId: "' + fiipsConstructionId + '",  fiipsDesignId: "' + fiipsDesignId + '", fiipsStructureId: "' + fiipsStructureId + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.success) {
                        window.open(urlSettings.download);
                        $("#localLetAgreeToTermsOfUse").prop("checked", false);
                        $("#stateLetAgreeToTermsOfUse").prop("checked", false);
                        toggleGetPlan();
                    }
                    else {
                        alert("Unable to find plan. Please contact the WisDOT Bureau of Structures.");
                    }
                }
            });
    });
    $("#clearFiipsConstructionId").click(function () {
        $("#fiipsConstructionId").val("");
        fiipsConstructionId = "";
        validFiipsConstructionId = false;
        $("#acceptImageDiv").hide();
        $("#rejectImageDiv").hide();
        $("#ImageDiv1").hide();
        $("#ImageDiv2").hide();
        toggleGetPlan();
    });
    $("#clearFiipsDesignId").click(function () {
        $("#fiipsDesignId").val("");
        fiipsDesignId = "";
        validFiipsDesignId = false;
        toggleGetPlan();
    });
    $("#clearFiipsStructureId").click(function () {
        $("#fiipsStructureId").val("");
        fiipsStructureId = "";
        validStructureId = true;
        $("#acceptImageDivS").hide();
        $("#rejectImageDivS").hide();
        $("#ImageDivSA").hide();
        $("#ImageDivSR").hide();
        toggleGetPlan();
    });
    $("#fiipsConstructionId").on("change", function (event) {
        //alert(event.type);
        validateFiipsConstructionId();
    });
    $("#validateFiipsConstructionId").click(function (event) {
        //alert(event.type);
        validateFiipsConstructionId();
    });

    $("#fiipsDesignId").on("change", function (event) {
        fiipsDesignId = $("#fiipsDesignId").val().trim();
    });
    $("#validateFiipsDesignId").click(function (event) {
        fiipsDesignId = $("#fiipsDesignId").val().trim();
    });

    $("#fiipsStructureId").on("change", function (event) {
        fiipsStructureId = $("#fiipsStructureId").val().trim();
        validateStructureId();
    });
    $("#validateFiipsStructureId").click(function (event) {
        fiipsStructureId = $("#fiipsStructureId").val().trim();
        validateStructureId();
    });

    $("#stateLetAgreeToTermsOfUse").click(function () {
        toggleGetPlan();
    });
    $("#localLetAgreeToTermsOfUse").click(function () {
        toggleGetPlan();
    });
    $("#stateLetAgreement").click(function () {
        window.open(urlSettings.getReport, "_blank");
    });
    $("#localLetAgreement").click(function () {
        window.open(urlSettings.getReport, "_blank");
    });

    function validateFiipsConstructionId() {
        $("#acceptImageDiv").hide();
        $("#rejectImageDiv").hide();
        $("#ImageDiv1").hide();
        $("#ImageDiv2").hide();

        fiipsConstructionId = $("#fiipsConstructionId").val().trim();
        var search = "-";
        var replaceWith = "";
        fiipsConstructionId = fiipsConstructionId.split(search).join(replaceWith);
            $.ajax({
                type: "POST",
                url: urlSettings.validateFiips,
                data: '{fiipsConstructionId: "' + fiipsConstructionId + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.success) {
                        $("#acceptImageDiv").show();
                        $("#ImageDiv1").show();
                        validFiipsConstructionId = true;
                    }
                    else {
                        $("#rejectImageDiv").show();
                        $("#ImageDiv2").show();
                        validFiipsConstructionId = false;
                    }
                    toggleGetPlan();
                }
            });
    }

    function enableLocalLet() {
        $.ajax({
            type: "POST",
            url: urlSettings.enableLocalLet,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.success) {
                    $("#projectTypeLocalLet").prop("disabled", false);
                }
                else {
                    $("#projectTypeLocalLet").prop("disabled", true);
                }
            }
        });
    }

    function validateStructureId() {
        $("#acceptImageDivS").hide();
        $("#rejectImageDivS").hide();
        $("#ImageDivSA").hide();
        $("#ImageDivSR").hide();

        fiipsStructureId = $("#fiipsStructureId").val().trim();
        var search = "-";
        var replaceWith = "";
        fiipsStructureId = fiipsStructureId.split(search).join(replaceWith);

        if (fiipsStructureId.length == 0) {
            $("#acceptImageDivS").hide();
            $("#ImageDivSA").hide();
            validStructureId = true;
        }
        else if (fiipsStructureId.length == 7 || fiipsStructureId.length == 11) {
            $("#acceptImageDivS").show();
            $("#ImageDivSA").show();
            validStructureId = true;
        }
        else {
            $("#rejectImageDivS").show();
            $("#ImageDivSR").show();
            validStructureId = false;
        }
        toggleGetPlan();
    }

    function toggleGetPlanOld() {
        if (validFiipsConstructionId) {
            if ($("#projectTypeStateLet").is(":checked") && $("#stateLetAgreeToTermsOfUse").prop("checked")) {
                $("#getPlan").prop("disabled", false);
            }
            else {

            }
            if ($("#projectTypeLocalLet").is(":checked") && $("#localLetAgreeToTermsOfUse").prop("checked")) {
                $("#getPlan").prop("disabled", false);
            }
            else {

            }
        }
        else {
            $("#getPlan").prop("disabled", true);
        }
    }

    function toggleGetPlanOldOld() {
        if (validFiipsConstructionId) {
            if ($("#projectTypeStateLet").is(":checked")) {
                if ($("#stateLetAgreeToTermsOfUse").prop("checked")) {
                    $("#getPlan").prop("disabled", false);
                }
                else {
                    $("#getPlan").prop("disabled", true);
                }
            }
            else if ($("#projectTypeLocalLet").is(":checked")) {
                if ($("#localLetAgreeToTermsOfUse").prop("checked")) {
                    $("#getPlan").prop("disabled", false);
                }
                else {
                    $("#getPlan").prop("disabled", true);
                }
            }
        }
        else {
            $("#getPlan").prop("disabled", true);
        }
    }

    function toggleGetPlan() {
        if ($("#projectTypeStateLet").is(":checked")) {
            if (validFiipsConstructionId && validStructureId && $("#stateLetAgreeToTermsOfUse").prop("checked")) {
                $("#getPlan").prop("disabled", false);
            }
            else {
                $("#getPlan").prop("disabled", true);
            }
        }
        else if ($("#projectTypeLocalLet").is(":checked")) {
            if ($("#localLetAgreeToTermsOfUse").prop("checked")) {
                $("#getPlan").prop("disabled", false);
            }
            else {
                $("#getPlan").prop("disabled", true);
            }
        }
    }

    // requires jquery-ui
    $("#planSearchOld").submit(function (event) {
        $("#ajaxLoaderDiv").show();
        event.preventDefault();
        var form = $(this);
        // TODO: Validate
        // Submit form
        $("#downloads").load(form.attr("action"), form.serialize());
        $("#ajaxLoaderDiv").hide();
    });

    $("#getPlanOld").click(function (event) {
        $("#ajaxLoaderDiv").show();
        window.open("/Home/GetPlanZip", "_blank");
        $("#ajaxLoaderDiv").hide();
    });
});

