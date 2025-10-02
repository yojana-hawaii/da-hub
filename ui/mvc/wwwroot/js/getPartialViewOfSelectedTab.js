function refreshTab(activeTab) {
    switch (activeTab) {
        case 'Department-Tab': $("#DepartmentPanel").load('/Lookup/Department'); break;
        case 'JobTitle-Tab': $("#JobTitlePanel").load('/Lookup/JobTitle'); break;
        case 'Location-Tab': $("#LocationPanel").load('/Lookup/Location'); break;
        default: break;
    };
    return;
};

$('.nav-link').click(function () {
    var activeTab = $(this).attr("id");
    refreshTab(activeTab);
})
