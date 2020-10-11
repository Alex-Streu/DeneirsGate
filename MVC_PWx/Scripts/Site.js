/* HELPERS */
function isEmpty(val) {
    if (val == undefined || val == null || val == '') { return true; }

    return false;
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

function ajaxPost(postData, url, successFunction, errorFunction) {
    if (errorFunction == null) { errorFunction = defaultErrorFunction; }

    $.ajax({
        type: "POST",
        url: url,
        data: JSON.stringify(postData),
        dataType: 'json',
        contentType: 'application/json',
        success: successFunction,
        error: errorFunction
    })
}

function defaultErrorFunction(error) {
    Notiflix.NotifyContent.Failure(error.responseText);
}

jQuery.fn.removeClassExcept = function (val) {
    return this.each(function () {
        $(this).removeClass().addClass(val);
    });
};

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

$('input[type="file"]').change(function () {
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
                    parent.find('img').attr('src', data.imagePath.replace('~', ''));
                    parent.find('.image-name').val(data.image);
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