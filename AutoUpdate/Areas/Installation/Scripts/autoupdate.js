$(function () {
    $('[data-autoupdate-check-url]').each(function () {
        var container = $(this);
        var url = container.attr('data-autoupdate-check-url');
        if (url != null) {
            $.getJSON(url, function (data) {
                if (data.UpdateAvailable) {
                    var upgradeMessage = container.find('.template').tmpl(data).prependTo(container);
                    container.find('a.update').click(function () {
                        var upgradeUrl = container.attr('data-autoupdate-upgrade-url');
                        $.getJSON(upgradeUrl, function (data) {
                            if (data.Success) {
                                upgradeMessage.remove();
                                container.find('.success-template').tmpl(data).prependTo(container);
                            }
                        });
                    });
                    container.fadeIn();
                }
            });
        }
    });
});