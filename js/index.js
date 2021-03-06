var loaderHtml = '<center style="max-width: 550px; margin-top: 10px"><div class="loader"></div></center>';
function loadActions(name = "") {
    $('.actionRow').remove();

    $("#main").append("<div id='actionRow' class='row actionRow'></div>");
    $("#actionRow").append(loaderHtml);
    actions.forEach(function (action, index) {
        if (name) {
            if (!action.Name.toLowerCase().includes(name.toLowerCase())) {
                return;
            }
        }
        var element = $("#template").clone().appendTo('#actionRow');
        element.attr('id', 'action-' + action.ID);
        var eleHtml = element.html();
        eleHtml = eleHtml.replace("{0}", action.Name);
        eleHtml = eleHtml.replace("{1}", action.Category);
        eleHtml = eleHtml.replace("{2}", action.Cooldown);
        eleHtml = eleHtml.replace("{3}", action.Description);
        eleHtml = eleHtml.replace("{4}", action.ID);
        element.html(eleHtml);
        element.show();
    });
    $('.loader').remove();
}

function loadTraits(name = "") {
    $('.actionRow').remove();

    $("#main").append("<div id='actionRow' class='row actionRow'></div>");
    $("#actionRow").append(loaderHtml);
    
    traits.forEach(function (action, index) {
        if (name) {
            if (!action.Name.toLowerCase().includes(name)) {
                return;
            }
        }
        var element = $("#template").clone().appendTo('#actionRow');
        element.attr('id', 'action-' + action.key);
        var eleHtml = element.html();
        eleHtml = eleHtml.replace("{0}", action.Name);
        eleHtml = eleHtml.replace("{1}", action.Description);
        eleHtml = eleHtml.replace("{2}", action.Level);
        eleHtml = eleHtml.replace("{3}", action.ClassJob);
        eleHtml = eleHtml.replace("{4}", action.key);
        element.html(eleHtml);
        element.show();
    });
    $('.loader').remove();
}