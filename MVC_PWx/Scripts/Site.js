/* GLOBAL VARIABLES */
var customErrorAction = "/Errors/CustomError";
var error500Action = "/Errors/Error500";
var error400Action = "/Errors/Error400";
var getUserTutorialAction = "/Tutorial/GetUserTutorial";
var updateUserTutorialAction = "/Tutorial/UpdateUserTutorial";

/* HELPERS */
function isEmpty(val) {
    if (val == undefined || val == null || val == '') { return true; }

    return false;
}

function createGuid() {
    function _p8(s) {
        var p = (Math.random().toString(16) + "000000000").substr(2, 8);
        return s ? "-" + p.substr(0, 4) + "-" + p.substr(4, 4) : p;
    }
    return _p8() + _p8(true) + _p8(true) + _p8();
}

function sortOptions(id) {
    var options = $(`${id} option`);
    var arr = options.map(function (_, o) { return { t: $(o).text(), v: o.value }; }).get();
    arr.sort(function (o1, o2) {
        var t1 = o1.t.toLowerCase(), t2 = o2.t.toLowerCase();

        return t1 > t2 ? 1 : t1 < t2 ? -1 : 0;
    });
    options.each(function (i, o) {
        o.value = arr[i].v;
        $(o).text(arr[i].t);
    });
}

$('.modal').on('shown.bs.modal', function () {
    $(this).find('[autofocus]').focus();
});

function promptDelete(callback) {
    $('#deleteContentBtn').attr('onclick', callback);
    $('#deleteContentModal').modal('show');
}

function loadEncounter(id) {
    if (id == undefined) { id = null; }

    if (!isEmpty(id) && id.IsEdited == true) {
        $("#encounterBody").load(`/Event/_EncounterPost`, id,
            function (response, status, xhr) {
                $('#encounterModal').modal('show');
            }
        );
    }
    else {
        $("#encounterBody").load(`/Event/_Encounter?id=${id}`,
            function (response, status, xhr) {
                $('#encounterModal').modal('show');
            }
        );
    }
}

AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};

function ajaxPost(postData, url, successFunction, errorFunction, completeFunction) {
    if (successFunction == null) { successFunction = defaultCompleteFunction; }
    if (errorFunction == null) { errorFunction = defaultErrorFunction; }
    if (completeFunction == null) { completeFunction = defaultCompleteFunction; }

    $.ajax({
        type: "POST",
        url: url,
        data: JSON.stringify(postData),
        dataType: 'json',
        contentType: 'application/json',
        success: successFunction,
        error: errorFunction,
        complete: completeFunction
    })
}

function defaultErrorFunction(error) {
    if (error.responseText.indexOf('<!DOCTYPE html>') > -1) {
        window.location = error500Action;
    }
    Notiflix.NotifyContent.Failure(error.responseText);
}

function defaultCompleteFunction() {
    //Prevent errors
}

jQuery.fn.removeClassExcept = function (val) {
    return this.each(function () {
        $(this).removeClass().addClass(val);
    });
};

$('.btn-suggestion').parent().mouseenter(function () {
    $(this).find('.btn-suggestion').show();
})

$('.btn-suggestion').parent().mouseleave(function () {
    $(this).find('.btn-suggestion').hide();
})

function suggest(fieldId, type, editor) {
    var postData = {
        Type: type
    }

    ajaxPost(postData, '/Suggestion/GenerateSuggestion', function (data) {
        if (data.success) {
            if (editor == null) {
                $(`#${fieldId}`).val(data.data);
            } else {
                editor.setData(data.data);
            }
        }
        else {
            Notiflix.NotifyContent.Failure(data.message);
        }
    })
}

/* TUTORIALS */
var tutorialKey = '';
var tutorialName = '';
var tutorialEngine = null;
var tutorialEngineSteps = [];

function initializeTutorial() {
    tutorialEngine = new EnjoyHint({
        onSkip: storeUserTutorial,
        onEnd: completeUserTutorial
    });

    $('#helpBtn').show();
}

function setTutorialSteps(steps) {
    tutorialEngineSteps = steps;
    tutorialEngine.set(steps);
}

$('#helpBtn').click(function () {
    initializeTutorial();
    setTutorialSteps(tutorialEngineSteps);
    tutorialEngine.resume();
})

$(document).on('click', '.enjoyhint_close_btn', function () {
    completeUserTutorial();
})

function getUserTutorial(name, callbackIfComplete) {
    tutorialName = name;
    var routes = window.location.pathname.split('/');
    var route = '';
    for (i = 0; i < routes.length; i++) {
        route += "/";
        if (routes[i].indexOf('-') > -1 && routes[i].length == '36') {
            break;
        }

        route += routes[i];
    }

    var postData = {
        Route: route,
        Name: name
    }
    
    var cookie = Cookies.get(postData.Route + postData.Name);
    if (cookie == 'true' && callbackIfComplete != null) {
        callbackIfComplete();
        return;
    }

    ajaxPost(postData, getUserTutorialAction, function (data) {
        if (data.success) {
            tutorialKey = data.data.TutorialKey;
            //Cookies.set(postData.Route + postData.Name, data.data.IsComplete ? 'true' : 'false');
            if (!data.data.IsComplete) {
                tutorialEngine.setCurrentStep(data.data.LastStep);
                tutorialEngine.resume();
            }
            else if (callbackIfComplete != null) {
                callbackIfComplete();
            }
        } else {
            Notiflix.NotifyContent.Failure(data.message);
        }
    })
}

function completeUserTutorial() {
    updateUserTutorial(true, 0);
}

function storeUserTutorial() {
    updateUserTutorial(false, tutorialEngine.getCurrentStep());
}

function updateUserTutorial(isComplete, lastStep) {
    var routes = window.location.pathname.split('/');
    var route = '';
    for (i = 0; i < routes.length; i++) {
        route += "/";
        if (routes[i].indexOf('-') > -1 && routes[i].length == '36') {
            break;
        }

        route += routes[i];
    }
    //Cookies.set(route + tutorialName, isComplete ? 'true' : 'false');

    var postData = {
        TutorialKey: tutorialKey,
        IsComplete: isComplete,
        LastStep: lastStep
    }

    ajaxPost(postData, updateUserTutorialAction, function (data) {
        if (data.success) {
            if (isComplete) {
                
            }
        } else {
            Notiflix.NotifyContent.Failure(data.message);
        }
    })
}

/**
 * Returns a number whose value is limited to the given range.
 *
 * Example: limit the output of this computation to between 0 and 255
 * (x * 255).clamp(0, 255)
 *
 * @param {Number} min The lower boundary of the output range
 * @param {Number} max The upper boundary of the output range
 * @returns A number in the range [min, max]
 * @type Number
 */
Number.prototype.clamp = function (min, max) {
    return Math.min(Math.max(this, min), max);
};


/* FANCY ITEMS */
$('.fancy-textarea textarea').focus(function () {
    $(this).parent().find('label').addClass('active');
})

$('.fancy-textarea textarea').blur(function () {
    $(this).parent().find('label').removeClass('active');
})

$('.fancy-dropdown select').focus(function () {
    $(this).parent().find('label').addClass('active');
})

$('.fancy-dropdown select').blur(function () {
    $(this).parent().find('label').removeClass('active');
})

$('body').on('click', '.fancy-tab-item', function () {
    var tab = $(this).data('tab');
    var panes = $(this).parent().data('panes');

    $(`#${panes}`).find('.fancy-tab-pane').each(function () {
        $(this).removeClass('active');
    });

    $(this).parent().find('.fancy-tab-item').each(function () {
        $(this).removeClass('active');
    });

    $(`#${tab}`).addClass('active');
    $(this).addClass('active');
})

$(document).ready(function () {
    $('.fancy-switch').each(function () {
        if ($(this).data('val') == 'True') {
            $(this).find('input').attr('checked', 'checked');
        }
    })
})


/* IMAGE UPLOADING */
var uploadImageUtilityAction = '/Utility/UploadImage/';
var deleteTempUtilityAction = '/Utility/DeleteTemp/';
var saveTempImageUtilityAction = '/Utility/SaveTempImage/';
var imageUploadCampaignKey = null;

$('.upload-image .overlay').click(function () {
    var input = $(this).parent().find('input[type="file"]');
    input.click();
})

$('.image-upload').click(function () {
    var input = $(this).parent().find('input[type="file"]');
    input.click();
})

$('input.user-image[type="file"]').change(function () {
    if ($(this).val() == null) { return; }

    uploadImage(this, null, null, true);
});

/** @description Uploads an image to the server for the given campaign.
 * @param {any} fileElement The input file element.
 * @param {string} folder The folder directory for the file.
 * @param {string} name The name of the file.
 * @param {boolean} isTemp Whether the image should be uploaded to the temp folder.
 * @return {number}
 */
function uploadImage(fileElement, folder, name, isTemp = false) {
    var image = $(fileElement).parent().find('img');
    uploadImageExternal(fileElement, folder, name, image, isTemp);
}

function uploadImageExternal(fileElement, folder, name, imageElement, isTemp = false) {
    var f = $(fileElement).prop('files')[0];
    if (f == undefined) { return; }

    var parent = $(fileElement).parent();
    var reader = new FileReader();
    imageUploadCampaignKey = $(parent).data('campaign');

    reader.onloadend = function () {
        var data = reader.result;
        var type = base64MimeType(data);
        if (type == null) { return; }

        var postData = {
            CampaignKey: imageUploadCampaignKey,
            Folder: folder,
            Name: name,
            File: data,
            FileType: type,
            IsTemp: isTemp
        }

        $.ajax({
            type: "POST",
            url: uploadImageUtilityAction,
            data: JSON.stringify(postData),
            dataType: 'json',
            contentType: 'application/json',
            processData: false,
            success: function (data) {
                if (data.success) {
                    $(imageElement).attr('src', data.imagePath.replace('~', ''));
                    $(parent).find('.image-name').val(data.image);
                    $(parent).find('.image-name').change();
                }
            },
            error: function (error) {
            }
        })
    }
    reader.readAsDataURL(f);
}

function base64MimeType(encoded) {
    var result = null;

    if (typeof encoded !== 'string') {
        return result;
    }

    var mime = encoded.match(/data:([a-zA-Z0-9]+\/[a-zA-Z0-9-.+]+).*,.*/);

    if (mime && mime.length) {
        result = mime[1];
    }

    var type = null;
    switch (result.replace('image/', '')) {
        case 'png': type = '.png'; break;
        case 'jpg': type = '.jpg'; break;
        case 'jpeg': type = '.jpg'; break;
    }

    return type;
}

$(window).on('unload', function (e) {
    if (imageUploadCampaignKey == null) { return; }

    var data = '?campaignKey=' + imageUploadCampaignKey;

    navigator.sendBeacon(deleteTempUtilityAction + data);
})