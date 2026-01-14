$(function () {
    var l = abp.localization.getResource('Examination');
    var service = dATERP.examination.examination.examSubject;

    // Initialize Admin Table with the shared component
    var dataTable = initAdminTable({
        tableId: 'ExamSubjectsTable',
        createButtonId: 'NewExamSubjectButton',
        ajaxUrl: '/Examination/ExamSubjects?handler=GetList',
        columns: [
            {
                data: 'name',
                title: l('Name'),
                orderable: true
            },
            {
                data: 'code',
                title: l('Code'),
                orderable: true
            },
            {
                data: 'description',
                title: l('Description'),
                orderable: false
            }
        ],
        createModal: {
            path: abp.appPath + 'Examination/ExamSubjects/CreateModal'
        },
        editModal: {
            path: abp.appPath + 'Examination/ExamSubjects/EditModal'
        },
        deleteService: service.delete,
        deleteConfirmMessage: l('ExamSubjectDeletionConfirmationMessage'),
        order: [[0, 'asc']]  // Sort by Name column (first column)
    });
});
