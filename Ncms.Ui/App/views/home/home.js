define(['plugins/router'], function (router) {
    var vm = {
        activate: function (query) {
            var redir = query ? query['redir'] : null;
            if (redir && !redir.includes("http")) {
                window.history.replaceState({}, window.document.title, '/');
                router.navigate(redir);
            }
        }
    };

    return vm;
});