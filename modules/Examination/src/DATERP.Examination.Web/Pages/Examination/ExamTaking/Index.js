$(function () {
    // State management
    let currentTaskIndex = -1; // -1 means overview
    let taskStates = {}; // {taskId: 'completed' | 'review' | null}
    const tasks = [];

    // Initialize tasks from DOM
    $('.tab[data-task-id]').each(function () {
        const taskId = $(this).data('task-id');
        const taskIndex = $(this).data('task-index');
        tasks.push({ id: taskId, index: taskIndex });
        taskStates[taskId] = null;
    });

    // Tab navigation
    $('.tab').on('click', function () {
        const tabType = $(this).data('tab');

        if (tabType === 'overview') {
            showOverview();
        } else {
            const taskIndex = $(this).data('task-index');
            showTask(taskIndex);
        }
    });

    function showOverview() {
        currentTaskIndex = -1;
        updateActiveTab();
        $('#overview-content').show();
        $('.task-detail-content').remove();
        updateNavigationButtons();
    }

    function showTask(index) {
        if (index < 0 || index >= tasks.length) return;

        currentTaskIndex = index;
        updateActiveTab();

        // Hide overview, show task detail
        $('#overview-content').hide();
        $('.task-detail-content').remove();

        const task = tasks[index];
        const taskTab = $(`.tab[data-task-id="${task.id}"]`);
        const taskContent = $(`.task-overview-item[data-task-id="${task.id}"] .task-preview`).text();
        const taskPoints = $(`.task-overview-item[data-task-id="${task.id}"] .task-points`).text();

        const detailHtml = `
            <div class="task-detail-content">
                <p>${taskContent}</p>
                <div class="mt-3">
                    <span class="badge bg-success">${taskPoints}</span>
                </div>
            </div>
        `;

        $('#instruction-content').append(detailHtml);
        updateNavigationButtons();
    }

    function updateActiveTab() {
        $('.tab').removeClass('active');

        if (currentTaskIndex === -1) {
            $('#tab-overview').addClass('active');
        } else {
            $(`.tab[data-task-index="${currentTaskIndex}"]`).addClass('active');
        }
    }

    function updateNavigationButtons() {
        const prevBtn = $('#prev-task');
        const nextBtn = $('#next-task');
        const markCompletedBtn = $('#mark-completed');
        const markReviewBtn = $('#mark-review');

        // Previous button
        if (currentTaskIndex <= 0) {
            prevBtn.prop('disabled', true);
        } else {
            prevBtn.prop('disabled', false);
        }

        // Next button
        if (currentTaskIndex >= tasks.length - 1) {
            nextBtn.prop('disabled', true);
        } else {
            nextBtn.prop('disabled', false);
        }

        // Mark buttons - disable in overview
        if (currentTaskIndex === -1) {
            markCompletedBtn.prop('disabled', true);
            markReviewBtn.prop('disabled', true);
        } else {
            markCompletedBtn.prop('disabled', false);
            markReviewBtn.prop('disabled', false);
        }
    }

    // Navigation buttons
    $('#prev-task').on('click', function () {
        if (currentTaskIndex > 0) {
            showTask(currentTaskIndex - 1);
        } else if (currentTaskIndex === 0) {
            showOverview();
        }
    });

    $('#next-task').on('click', function () {
        if (currentTaskIndex === -1 && tasks.length > 0) {
            showTask(0);
        } else if (currentTaskIndex < tasks.length - 1) {
            showTask(currentTaskIndex + 1);
        }
    });

    // Mark completed
    $('#mark-completed').on('click', function () {
        if (currentTaskIndex === -1) return;

        const task = tasks[currentTaskIndex];
        taskStates[task.id] = 'completed';
        updateTaskStatus(task.id, 'completed');
        updateProgress();

        // Auto-advance to next task
        if (currentTaskIndex < tasks.length - 1) {
            showTask(currentTaskIndex + 1);
        }
    });

    // Mark for review
    $('#mark-review').on('click', function () {
        if (currentTaskIndex === -1) return;

        const task = tasks[currentTaskIndex];
        taskStates[task.id] = 'review';
        updateTaskStatus(task.id, 'review');
    });

    function updateTaskStatus(taskId, status) {
        const tab = $(`.tab[data-task-id="${taskId}"]`);
        const statusIcon = $(`#status-${taskId}`);

        tab.removeClass('completed review');

        if (status === 'completed') {
            tab.addClass('completed');
            statusIcon.html('<i class="ti ti-circle-check" style="color: #00b894;"></i>');
        } else if (status === 'review') {
            tab.addClass('review');
            statusIcon.html('<i class="ti ti-flag" style="color: #fdcb6e;"></i>');
        } else {
            statusIcon.html('<i class="ti ti-circle" style="color: #ccc;"></i>');
        }
    }

    function updateProgress() {
        const completedCount = Object.values(taskStates).filter(s => s === 'completed').length;
        const totalCount = tasks.length;
        const percentage = totalCount > 0 ? (completedCount / totalCount) * 100 : 0;

        $('#completed-count').text(completedCount);
        $('#progress-bar').css('width', percentage + '%');
    }

    // Project change
    $('#project-select').on('change', function () {
        const projectId = $(this).val();
        const urlParams = new URLSearchParams(window.location.search);
        const examListId = urlParams.get('examListId');
        window.location.href = `/Examination/ExamTaking?examListId=${examListId}&projectId=${projectId}`;
    });

    // Restart
    $('#restart-link').on('click', function () {
        if (confirm('Bạn có chắc muốn khởi động lại? Tất cả tiến độ sẽ bị mất.')) {
            taskStates = {};
            tasks.forEach(t => updateTaskStatus(t.id, null));
            updateProgress();
            showOverview();
        }
    });

    // Grade
    $('#grade-link, #submit-project-btn').on('click', function () {
        const completedCount = Object.values(taskStates).filter(s => s === 'completed').length;
        const totalCount = tasks.length;
        alert(`Điểm của bạn: ${completedCount}/${totalCount} nhiệm vụ hoàn thành`);
    });

    // Help
    $('#help-btn').on('click', function () {
        alert('Hướng dẫn:\n- Click vào các tab để xem từng nhiệm vụ\n- "Đánh dấu hoàn thành" khi làm xong\n- "Đánh dấu cần xem lại" nếu chưa chắc\n- Bấm "Nộp bài" khi hoàn tất');
    });

    // Initialize
    updateNavigationButtons();
});

// Global function for task click from overview
function goToTask(taskId, index) {
    $(`.tab[data-task-index="${index}"]`).click();
}
