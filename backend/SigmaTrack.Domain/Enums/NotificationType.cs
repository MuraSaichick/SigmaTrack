using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SigmaTrack.Domain.Enums
{
    public enum NotificationType
    {
        [Description("Назначен исполнитель на задачу")]
        IssueAssigned = 1,

        [Description("Изменен статус задачи")]
        IssueStatusChanged = 2,

        [Description("Добавлен новый комментарий")]
        NewComment = 3,

        [Description("Пользователь упомянут в комментариях/описании")]
        UserMentioned = 4,
        [Description("Получено приглашение в проект")]
        ProjectInvitationReceived = 10,

        [Description("Приглашение в проект принято")]
        ProjectInvitationAccepted = 11,

        [Description("Приглашение в проект отклонено")]
        ProjectInvitationRejected = 12,

        [Description("Пользователь удален из проекта")]
        ProjectMemberRemoved = 13,

        [Description("Изменена роль пользователя в проекте")]
        ProjectRoleChanged = 14,
        [Description("Спринт запущен")]
        SprintStarted = 20,

        [Description("Спринт успешно завершен")]
        SprintCompleted = 21,
        [Description("Общее системное уведомление")]
        SystemAlert = 90,
        [Description("Уведомление о технических работах")]
        SystemMaintenance = 91
    }
}