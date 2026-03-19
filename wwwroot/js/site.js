// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

/* ============================================================
   TaskFlow – site.js
   AJAX delete, toast notifications, file input enhancement,
   live search, sidebar stats
   ============================================================ */

$(function () {

    /* ── Update sidebar quick-stats ── */
    if (window.TASK_STATS) {
        $('#stat-pending').text(window.TASK_STATS.pending);
        $('#stat-inprogress').text(window.TASK_STATS.inProgress);
        $('#stat-completed').text(window.TASK_STATS.completed);
    }

    /* ── Custom file-input label update ── */
    $(document).on('change', '.custom-file-input', function () {
        var fileName = $(this).val().split('\\').pop() || 'Choose file…';
        $(this).closest('.custom-file').find('.custom-file-label').text(fileName);
    });

    /* ── Live table search ── */
    $('#tableSearch').on('input', function () {
        var q = $(this).val().toLowerCase().trim();
        var visible = 0;
        $('#tasksTable tbody .task-row').each(function () {
            var title = $(this).data('title') || '';
            var desc = $(this).find('.task-desc').text().toLowerCase();
            var match = title.includes(q) || desc.includes(q);
            $(this).toggle(match);
            if (match) visible++;
        });
        $('#rowCount').text(visible);
    });

    /* ── Delete button click → show modal ── */
    var pendingDeleteId = null;
    var pendingDeleteTitle = null;

    $(document).on('click', '.btn-delete', function () {
        pendingDeleteId = $(this).data('task-id');
        pendingDeleteTitle = $(this).data('task-title');
        $('#deleteTaskTitle').text(pendingDeleteTitle);
        $('#deleteModal').modal('show');
    });

    /* ── Confirm delete → AJAX call ── */
    $('#confirmDeleteBtn').on('click', function () {
        if (!pendingDeleteId) return;

        var $btn = $(this);
        $btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin m-1"></i>Deleting…');

        $.ajax({
            url: '/Tasks/Delete/' + pendingDeleteId,
            type: 'DELETE',
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (res) {
                $('#deleteModal').modal('hide');

                if (res.success) {
                    // Animate row out, then remove
                    var $row = $('#task-row-' + pendingDeleteId);
                    $row.addClass('deleting');
                    setTimeout(function () {
                        $row.remove();
                        updateRowCount();
                        updateEmptyState();
                    }, 420);
                    showToast(res.message, 'success');
                } else {
                    showToast(res.message || 'Could not delete task.', 'error');
                }
            },
            error: function (xhr) {
                $('#deleteModal').modal('hide');
                var msg = xhr.responseJSON?.message || 'An unexpected error occurred.';
                showToast(msg, 'error');
            },
            complete: function () {
                $btn.prop('disabled', false).html('<i class="fas fa-trash-alt m-1"></i>Yes, Delete');
                pendingDeleteId = null;
                pendingDeleteTitle = null;
            }
        });
    });

    /* ── Reset modal state on close ── */
    $('#deleteModal').on('hidden.bs.modal', function () {
        pendingDeleteId = null;
        pendingDeleteTitle = null;
    });

    /* ── Form submit loading state ── */
    $('#createTaskForm').on('submit', function () {
        var $btn = $('#submitBtn');
        $btn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin m-1"></i>Saving…');
    });

});

/* ── Update visible row count ── */
function updateRowCount() {
    var count = $('#tasksTable tbody .task-row:visible').length;
    $('#rowCount').text(count);
}

/* ── Show empty state if no rows ── */
function updateEmptyState() {
    var remaining = $('#tasksTable tbody .task-row').length;
    if (remaining === 0) {
        $('#tasksTable').closest('.table-responsive').replaceWith(
            '<div class="empty-state py-5 text-center">' +
            '<i class="fas fa-clipboard-list fa-4x text-muted mb-3"></i>' +
            '<p class="text-muted">All tasks deleted. Create a new one!</p>' +
            '</div>'
        );
        $('#rowCount').text('0');
    }
}

/* ── Toast notification ── */
function showToast(message, type) {
    var icons = {
        success: 'fa-check-circle',
        error: 'fa-times-circle',
        info: 'fa-info-circle'
    };
    var icon = icons[type] || icons.info;

    var $toast = $('<div class="tf-toast toast-' + type + '" role="alert">' +
        '<span class="toast-icon"><i class="fas ' + icon + '"></i></span>' +
        '<span class="toast-body">' + $('<span>').text(message).html() + '</span>' +
        '<span class="toast-close" title="Dismiss"><i class="fas fa-times"></i></span>' +
        '</div>');

    $('#toast-container').append($toast);

    // Close button
    $toast.find('.toast-close').on('click', function () {
        dismissToast($toast);
    });

    // Auto-dismiss after 4s
    setTimeout(function () { dismissToast($toast); }, 4000);
}

function dismissToast($toast) {
    $toast.addClass('hiding');
    setTimeout(function () { $toast.remove(); }, 320);
}

// Make showToast globally accessible (used inline from Razor)
window.showToast = showToast;
