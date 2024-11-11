
$(document).ready(function () {
    checkOnLoadPage();
});


function checkOnLoadPage() {
    var currentPage = getCookie("current-page");

    if (currentPage != "") {
        indexAccessPage(currentPage);
    } else {
        indexAccessPage('dashboard');
    }
}
