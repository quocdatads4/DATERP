$(function () {
    var l = abp.localization.getResource('Examination');
    var service = dATERP.examination.examination.examProject;

    // Initialize Admin Table with the shared component
    var dataTable = initAdminTable({
        tableId: 'ExamProjectsTable',
        createButtonId: 'NewExamProjectButton',
        ajaxUrl: '/Examination/ExamProjects?handler=GetList',
        columns: [
            {
                data: 'name',
                title: l('ProjectName'),
                orderable: true
            },
            {
                data: 'examListTitle',
                title: l('ExamListTitle'),
                orderable: true
            },
            {
                data: 'instruction',
                title: l('Instruction'),
                orderable: false,
                render: function (data) {
                    if (!data) return '';
                    return data.length > 50 ? data.substring(0, 50) + '...' : data;
                }
            },
            {
                data: 'order',
                title: l('Order'),
                orderable: true
            }
        ],
        createModal: {
            path: abp.appPath + 'Examination/ExamProjects/CreateModal'
        },
        editModal: {
            path: abp.appPath + 'Examination/ExamProjects/EditModal'
        },
        deleteService: service.delete,
        deleteConfirmMessage: l('ExamProjectDeletionConfirmationMessage'),
        order: [[3, 'asc']]  // Sort by Order column
    });
});
