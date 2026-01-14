$(function () {
    var l = abp.localization.getResource('Examination');
    var service = dATERP.examination.examination.examTask;

    // Initialize Admin Table with the shared component
    var dataTable = initAdminTable({
        tableId: 'ExamTasksTable',
        createButtonId: 'NewExamTaskButton',
        ajaxUrl: '/Examination/ExamTasks?handler=GetList',
        columns: [
            {
                data: 'content',
                title: l('Content'),
                orderable: true,
                render: function (data) {
                    if (!data) return '';
                    return data.length > 80 ? data.substring(0, 80) + '...' : data;
                }
            },
            {
                data: 'projectName',
                title: l('ProjectName'),
                orderable: true
            },
            {
                data: 'point',
                title: l('Point'),
                orderable: true,
                render: function (data) {
                    return data + ' ' + l('Points');
                }
            },
            {
                data: 'order',
                title: l('Order'),
                orderable: true
            }
        ],
        createModal: {
            path: abp.appPath + 'Examination/ExamTasks/CreateModal'
        },
        editModal: {
            path: abp.appPath + 'Examination/ExamTasks/EditModal'
        },
        deleteService: service.delete,
        deleteConfirmMessage: l('ExamTaskDeletionConfirmationMessage'),
        order: [[3, 'asc']]  // Sort by Order column
    });
});
