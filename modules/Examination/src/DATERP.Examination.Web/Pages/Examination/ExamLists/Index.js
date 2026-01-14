$(function () {
    var l = abp.localization.getResource('Examination');
    var service = dATERP.examination.examination.examList;

    // Initialize Admin Table with the shared component
    var dataTable = initAdminTable({
        tableId: 'ExamListsTable',
        createButtonId: 'NewExamListButton',
        ajaxUrl: '/Examination/ExamLists?handler=GetList',
        columns: [
            {
                data: 'title',
                title: l('Title'),
                orderable: true
            },
            {
                data: 'subjectName',
                title: l('SubjectName'),
                orderable: true
            },
            {
                data: 'timeLimit',
                title: l('TimeLimit'),
                orderable: true,
                render: function(data) {
                    return data + ' ' + l('Minutes');
                }
            },
            {
                data: 'order',
                title: l('Order'),
                orderable: true
            }
        ],
        createModal: {
            path: abp.appPath + 'Examination/ExamLists/CreateModal'
        },
        editModal: {
            path: abp.appPath + 'Examination/ExamLists/EditModal'
        },
        deleteService: service.delete,
        deleteConfirmMessage: l('ExamListDeletionConfirmationMessage'),
        order: [[3, 'asc']]  // Sort by Order column
    });
});
