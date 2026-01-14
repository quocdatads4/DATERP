// Wait for document ready
document.addEventListener('DOMContentLoaded', function () {
    if (typeof jQuery === 'undefined') {
        console.error('FATAL: jQuery is NOT defined!');
        return;
    }

    if (typeof abp === 'undefined') {
        console.error('FATAL: abp is NOT defined!');
        return;
    }

    (function ($) {
        var l = abp.localization.getResource('DATERP');
        var examSubjectService = dATERP.examination.examination.examSubject;

        var dataTable = $('#ExamSubjectsTable').DataTable(
            abp.libs.datatables.normalizeConfiguration({
                serverSide: true,
                paging: true,
                order: [[0, "asc"]],
                searching: true,
                scrollX: true,
                ajax: abp.libs.datatables.createAjax(examSubjectService.getList),
                columnDefs: [
                    {
                        title: l('SubjectName') || "Tên môn thi",
                        data: "name",
                        render: function (data, type, row) {
                            var letter = data ? data.charAt(0).toUpperCase() : '?';
                            return '<div class="d-flex align-items-center">' +
                                '<div class="avatar avatar-sm me-2 bg-label-info">' +
                                '<span class="avatar-initial rounded-circle">' + letter + '</span>' +
                                '</div>' +
                                '<div>' +
                                '<h6 class="mb-0 text-truncate" style="max-width: 200px;" title="' + data + '">' + data + '</h6>' +
                                '<small class="text-muted">' + (row.id ? row.id.substring(0, 8) : '') + '</small>' +
                                '</div>' +
                                '</div>';
                        }
                    },
                    {
                        title: l('SubjectCode') || "Mã môn",
                        data: "code",
                        render: function (data) {
                            return '<span class="badge bg-label-primary">' + (data || '') + '</span>';
                        }
                    },
                    {
                        title: l('CreationTime') || "Ngày tạo",
                        data: "creationTime",
                        render: function (data) {
                            if (!data) return '';
                            var dateVal = luxon.DateTime.fromISO(data);
                            return '<span class="text-muted"><i class="ti ti-calendar-event me-1"></i>' +
                                dateVal.toLocaleString(luxon.DateTime.DATE_MED) +
                                '</span>';
                        }
                    },
                    {
                        title: l('Actions') || "Thao tác",
                        rowAction: {
                            items: [
                                {
                                    text: l('Edit') || "Sửa",
                                    icon: "ti ti-edit",
                                    action: function (data) {
                                        abp.notify.info("Tính năng Sửa đang được phát triển!", "Thông báo");
                                    }
                                },
                                {
                                    text: l('Delete') || "Xóa",
                                    icon: "ti ti-trash",
                                    confirmMessage: function (data) {
                                        return (l('SubjectDeletionConfirmationMessage') || "Bạn có chắc chắn muốn xóa môn thi") + " " + data.record.name + "?";
                                    },
                                    action: function (data) {
                                        examSubjectService.delete(data.record.id)
                                            .then(function () {
                                                abp.notify.success(l('SuccessfullyDeleted') || "Đã xóa môn thi thành công.");
                                                dataTable.ajax.reload();
                                            });
                                    }
                                }
                            ]
                        }
                    }
                ]
            })
        );

        $('#NewExamSubjectButton').click(function (e) {
            e.preventDefault();
            // TODO: Implement Create Modal
            abp.notify.info("Tính năng đang được phát triển!", "Thông báo");
        });
    })(jQuery);
});
