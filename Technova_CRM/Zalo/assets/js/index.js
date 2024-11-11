function indexAccessPage(page, numb) {
    try {
        setCookie("current-page", page, 30);

        $('.menu').removeClass('active');

        var random = Math.random();
        var hdfID = $('#hdfViewDetailID').val();

        switch (page) {
            // Sales

            case 'sale-leads':
                $('#mnSales').addClass('active');
                $('#mnSaleLeads').addClass('active');

                var m = 'leads';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'sale-opportunities':
                $('#mnSales').addClass('active');
                $('#mnSaleOpportunities').addClass('active');

                var m = 'opportunities';
                var src = `app/${m}/${m}.html?var=${random}`;
                if (hdfID != '') {
                    src += `&ID=${hdfID}`;
                }
                document.getElementById('ivg-content').src = src;
                break;

            case 'sale-competitors':
                $('#mnSales').addClass('active');
                $('#mnSaleCompetitors').addClass('active');

                var m = 'competitors';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'sale-quotes':
                $('#mnSales').addClass('active');
                $('#mnSaleQuotes').addClass('active');

                var m = 'quotes';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'sale-orders':
                $('#mnSales').addClass('active');
                $('#mnSaleOrders').addClass('active');

                var m = 'orders';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'sale-invoices':
                $('#mnSales').addClass('active');
                $('#mnSaleInvoices').addClass('active');

                var m = 'invoices';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'sale-dashboard':
                $('#mnSales').addClass('active');
                $('#mnSaleDashboard').addClass('active');

                var m = page;
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'sale-marketing':
                $('#mnSales').addClass('active');
                $('#mnSaleMarketing').addClass('active');

                var m = 'marketing';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'sale-campaigns':
                $('#mnSales').addClass('active');
                $('#mnSaleCampaigns').addClass('active');

                var m = 'campaigns';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            // Serivces
            case 'service-dashboard':
                $('#mnServices').addClass('active');
                $('#mnServDashboard').addClass('active');

                var m = 'service-dashboard';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'service-cases':
                $('#mnServices').addClass('active');
                $('#mnServCases').addClass('active');

                var m = 'cases';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'service-calendar':
                $('#mnServices').addClass('active');
                $('#mnServCalendar2').addClass('active');

                var m = 'service-calendar';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'service-contracts':
                $('#mnServices').addClass('active');
                $('#mnServContracts').addClass('active');

                var m = 'contracts';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'service-services':
                $('#mnServices').addClass('active');
                $('#mnServServices').addClass('active');

                var m = 'services';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;


            // Common
            case 'accounts':
                if (numb == 1) {
                    $('#mnSales').addClass('active');
                    $('#mnSaleAccounts').addClass('active');
                } else {
                    $('#mnServices').addClass('active');
                    $('#mnServAccounts').addClass('active');
                }

                var m = 'accounts';

                var src = `app/${m}/${m}.html?var=${random}`;
                if (hdfID != '') {
                    src += `&ID=${hdfID}`;
                }
                document.getElementById('ivg-content').src = src;
                break;

            case 'activities':
                if (numb == 1) {
                    $('#mnSales').addClass('active');
                    $('#mnSaleActivities').addClass('active');
                } else {
                    $('#mnServices').addClass('active');
                    $('#mnServActivities').addClass('active');
                }

                var m = 'activities';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'contacts':
                if (numb == 1) {
                    $('#mnSales').addClass('active');
                    $('#mnSaleContacts').addClass('active');
                } else {
                    $('#mnServices').addClass('active');
                    $('#mnServContacts').addClass('active');
                }

                var m = 'contacts';

                var src = `app/${m}/${m}.html?var=${random}`;
                if (hdfID != '') {
                    src += `&ID=${hdfID}`;
                }
                document.getElementById('ivg-content').src = src;
                break;

            case 'calendar':
                if (numb == 1) {
                    $('#mnSales').addClass('active');
                    $('#mnSaleCalendar').addClass('active');
                } else {
                    $('#mnServices').addClass('active');
                    $('#mnServCalendar1').addClass('active');
                }

                var m = 'calendar';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'dashboard':
                $('#mnDashboard').addClass('active');

                var m = page;
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'products':
                $('#mnSettings').addClass('active');
                $('#mnSettingCatalogs').addClass('active');
                $('#mnSettingProducts').addClass('active');

                var m = 'products';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'unit-groups':
                $('#mnSettings').addClass('active');
                $('#mnSettingCatalogs').addClass('active');
                $('#mnSettingUnitGroups').addClass('active');

                var m = 'unit-groups';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'price-lists':
                $('#mnSettings').addClass('active');
                $('#mnSettingCatalogs').addClass('active');
                $('#mnSettingPriceLists').addClass('active');

                var m = 'price-lists';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'discount-lists':
                $('#mnSettings').addClass('active');
                $('#mnSettingCatalogs').addClass('active');
                $('#mnSettingDiscountLists').addClass('active');

                var m = 'discount-lists';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'subjects':
                $('#mnSettings').addClass('active');
                $('#mnSettingManagements').addClass('active');
                $('#mnSettingSubjects').addClass('active');

                var m = 'subjects';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'reports':
                if (numb == 1) {
                    $('#mnSales').addClass('active');
                    $('#mnSaleReports').addClass('active');
                } else {
                    $('#mnServices').addClass('active');
                    $('#mnServReports').addClass('active');
                }

                var m = 'reports';
                document.getElementById('ivg-content').src = `app/${m}/${m}.html?var=${random}`;
                break;

            case 'logout':
                window.location.href = `app/${page}/${page}.html?var=${random}`;
                //document.getElementById('ivg-content').src = `app/${page}/${page}.html?var=${random}`;
                break;
        }

        $('#hdfViewDetailID').val('');

    } catch (e) {
        console.log(e);
    }
}


function indexPageContactDetail(id) {
    $('#hdfViewDetailID').val(id);
    $('#tabContact').click();
}


function indexPageAccountDetail(id) {
    $('#hdfViewDetailID').val(id);
    $('#tabAccount').click();
}


function indexPageOpportunityDetail(id) {
    $('#hdfViewDetailID').val(id);
    $('#tabOpportunity').click();
}


function index_switch_language(vn) {
    try {
        vn = vn != undefined ? true : false;
        if (vn) {
            //$('.btnLanguage').removeClass
        }
    } catch (e) {
    }
}