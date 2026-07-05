// D:/Projects/SigmaTrack/Project/frontend/i18n/i18n.config.ts
export default defineI18nConfig(() => ({
  legacy: false,
  defaultLocale: 'ru',
  detectBrowserLanguage: {
    useCookie: true,
    cookieKey: 'i18n_redirected',
    redirectOn: 'root'
  },
  messages: {
    ru: {
      auth: {
        project_name: 'SigmaTrack',
        tagline: 'Система управления проектами',
        login_title: 'Вход в аккаунт',
        register_title: 'Создание аккаунта',
        no_account: 'Еще нет аккаунта?',
        have_account: 'Уже есть аккаунт?',
        register_link: 'Зарегистрироваться',
        login_link: 'Войти',
        login_btn: 'Войти',
        register_btn: 'Создать аккаунт',
      },
      fields: {
        login: 'Логин',
        password: 'Пароль',
        firstname: 'Имя',
        patronymic: 'Отчество (необязательно)',
        lastname: 'Фамилия',
        email: 'Электронная почта',
        phone: 'Номер телефона'
      },
      menu: {
        "dashboard": "Обзор",
        "issues": "Задачи",
        "kanban": "Канбан",
        "sprints": "Спринты",
        "team": "Команда",
        "settings": "Настройки проекта",
        my_tasks: 'Мои задачи',
        board: 'Канбан-доска',
        reports: 'Аналитика',
        profile_settings: 'Настройки профиля',
        logout: 'Выйти'
      },
      globalRoles: {
        admin: 'Администратор',
        user: 'Разработчик'
      },
      projectRoles: {
        "1": "Менеджер проекта",
        "2": "Разработчик",
        "3": "QA Инженер",
        "4": "Наблюдатель"
      },
      projects: {
        placeholder: 'Выберите проект',
        select: 'Проект'
      },
      team: {
        title: 'Участники проекта',
        subtitle: 'Просмотр текущего состава команды, ролей и управление вашим доступом.',
        refreshBtn: 'Обновить',
        yourStatus: 'Ваш статус в проекте',
        youBadge: 'Вы',
        joinedAt: 'Присоединился(-ась)',
        leaveBtn: 'Покинуть проект',
        teamMembers: 'Состав команды ({count})',
        addedAt: 'Добавлен(-а)',
        empty: {
          title: 'Вы пока единственный участник проекта.',
          desc: 'Пригласите коллег во вкладке «Настройки проекта».'
        },
        modal: {
          title: 'Покинуть команду проекта?',
          desc: 'Это действие необратимо. Вы полностью потеряете доступ к баг-трекеру, канбан-доскам и отчётам этого проекта.',
          cancelBtn: 'Отмена',
          confirmBtn: 'Да, выйти'
        },
        toast: {
          cannotLeave: {
            title: 'Действие невозможно',
            desc: 'Вы являетесь Владельцем (Project Manager) этого проекта. Передайте права управления другому участнику в Настройках, прежде чем покинуть проект.'
          },
          error: {
            title: 'Ошибка',
            desc: 'Не удалось покинуть проект.'
          }
        }
      },
      projectSettings: {
        "title": "Настройки проекта",
        "subtitle": "Управление параметрами проекта, правами доступа команды и конфигурацией трекера.",
        "accessDenied": "Доступ ограничен",
        "accessDeniedDesc": "Только Владелец (Project Manager) имеет доступ к настройкам этого проекта.",
        "tabs": {
          "general": "Общие настройки",
          "members": "Управление командой",
          "danger": "Опасная зона"
        },
        "general": {
          "title": "Основная информация",
          "nameLabel": "Название проекта",
          "prefixLabel": "Префикс задач (Заглавные)",
          "descLabel": "Описание проекта",
          "descPlaceholder": "Краткое описание целей и задач команды...",
          "saveBtn": "Сохранить изменения"
        },
        "members": {
          "title": "Участники команды ({count})",
          "creatorBadge": "Создатель"
        },
        "danger": {
          "title": "Удаление проекта",
          "desc": "Будьте предельно осторожны. Это действие полностью удалит проект вместе со всеми спринтами, канбан-досками, задачами (багами) и файлами вложений. Данные невозможно будет восстановить.",
          "deleteBoxTitle": "Удалить этот проект",
          "projectNameLabel": "Имя проекта:",
          "deleteBtn": "Удалить проект"
        },
        "modals": {
          "cancel": "Отмена",
          "kick": {
            "title": "Исключить участника?",
            "desc": "Вы действительно хотите удалить пользователя {name} из проекта? Он потеряет доступ ко всем задачам и доскам.",
            "confirm": "Да, удалить"
          },
          "delete": {
            "title": "Удалить проект окончательно?",
            "desc": "Это действие невозможно отменить. Все спринты, задачи и вложения будут стёрты навсегда.",
            "inputLabel": "Для подтверждения введите имя проекта:",
            "inputPlaceholder": "Введите название проекта",
            "confirm": "Я понимаю, удалить проект"
          }
        },
        "notifications": {
          "nameMismatch": "Имя проекта введено неверно",
          "errorTitle": "Ошибка"
        }
      },
      profile: {
        title: 'Настройки профиля',
        view: 'Просмотр',
        edit: 'Редактировать',
        save: 'Сохранить изменения',
        cancel: 'Отмена',
        successUpdate: 'Профиль успешно обновлен!',
        fields: {
          firstname: 'Имя',
          lastname: 'Фамилия',
          patronymic: 'Отчество',
          phone: 'Телефон',
          statusMessage: 'Статус (где вы?)',
          statusPlaceholder: 'Например: В отпуске до 10.06',
          bio: 'О себе',
          position: 'Должность',
          department: 'Отдел / Департамент',
          skills: 'Навыки',
          birthDate: 'Дата рождения',
          telegram: 'Telegram',
          github: 'GitHub'
        },
        validation: {
          required: 'Это поле обязательно для заполнения'
        }
      },
      issues: {
        myActiveTitle: 'Мои активные задачи',
        myActiveSubtitle: 'Список всех незакрытых задач, назначенных на вас в текущих проектах.',
        total: 'Всего: {count}',
        emptyTitle: 'Активных задач нет',
        emptyDesc: 'Отличная работа! У вас нет открытых или требующих внимания задач.',
        updatedAt: 'Обновлено: {date}',
        loading: 'Загрузка…',
        editBtn: 'Редактировать',
        blockedAlertTitle: 'Задача заблокирована',
        blockedAlertDefaultDesc: 'Причина блокировки не указана.',
        description: 'Описание',
        descriptionEmpty: 'Описание отсутствует.',
        stepsToReproduce: 'Шаги для воспроизведения',
        reproducibleStable: 'Воспроизводится стабильно:',
        yes: 'Да',
        no: 'Нет',
        timeTracking: 'Оценка и логирование времени',
        timeEstimation: 'Оценка (ч)',
        timeLogged: 'Затрачено (ч)',
        timeRemaining: 'Осталось (ч)',
        linkedIssues: 'Связанные задачи',
        commentsTitle: 'Комментарии ({count})',
        commentsEmpty: 'Комментариев пока нет. Будьте первыми!',
        writeComment: 'Написать комментарий',
        commentPlaceholder: 'Оставьте ваш комментарий здесь...',
        internalCommentLabel: 'Внутренний комментарий (только для команды)',
        sendCommentBtn: 'Отправить',
        issueDetails: 'Детали задачи',
        fieldStatus: 'Статус',
        fieldAssignee: 'Исполнитель',
        assigneeNone: 'Не назначен',
        fieldType: 'Тип',
        fieldPriority: 'Приоритет',
        fieldSeverity: 'Серьезность',
        fieldReporter: 'Автор',
        fieldComponent: 'Компонент',
        fieldVersion: 'Версия',
        fieldFixVersion: 'Исправить в',
        fieldEnvironment: 'Окружение',
        fieldTags: 'Теги',
        created: 'Создана:',
        updated: 'Обновлена:',
        deadline: 'Дедлайн:',
        startedAt: 'В работе с:',
        resolvedAt: 'Решена:',
        closedAt: 'Закрыта:',
        newIssueTitle: 'Новая задача',
        newIssueSubtitle: 'Сформируйте описание и задайте параметры для новой таски',
        formTitleLabel: 'Название задачи',
        formTitlePlaceholder: 'Например: Разработать модуль авторизации через JWT',
        formDescLabel: 'Описание',
        formDescPlaceholder: 'Опишите контекст, требования или шаги для воспроизведения...',
        formTypeLabel: 'Тип задачи',
        formPriorityLabel: 'Приоритет',
        formAssigneePlaceholder: 'Не назначен',
        formTagsHint: 'Разделяйте их запятыми',
        formTagsPlaceholder: 'frontend, bug, sprint-2',
        cancelBtn: 'Отмена',
        createBtn: 'Создать задачу',
        severityLevels: {
          s4: 'S4 - Незначительный',
          s3: 'S3 - Умеренный',
          s2: 'S2 - Значительный',
          s1: 'S1 - Блокирующий',
          unknown: 'Уровень {level}'
        },
        types: {
          bug: 'Баг',
          feature: 'Фича',
          improvement: 'Улучшение',
          task: 'Задача'
        },
        priorities: {
          low: 'Низкий',
          medium: 'Средний',
          high: 'Высокий',
          critical: 'Критический'
        }
      }
    },
    en: {
      auth: {
        project_name: 'SigmaTrack',
        tagline: 'Project management system',
        login_title: 'Sign In',
        register_title: 'Create Account',
        no_account: "Don't have an account?",
        have_account: 'Already have an account?',
        register_link: 'Sign Up',
        login_link: 'Sign In',
      },
      fields: {
        login: 'Login',
        password: 'Password',
        firstname: 'First Name',
        patronymic: 'Patronymic (optional)',
        lastname: 'Last Name',
        email: 'Email Address',
        phone: 'Phone Number'
      },
      menu: {
        "dashboard": "Dashboard",
        "issues": "Issues",
        "kanban": "Kanban",
        "sprints": "Sprints",
        "team": "Team",
        "settings": "Project Settings",
        my_tasks: 'My Tasks',
        board: 'Board',
        reports: 'Reports',
        profile_settings: 'Profile Settings',
        logout: 'Logout'
      },
      globalRoles: {
        admin: 'Administrator',
        user: 'Developer'
      },
      projectRoles: {
        "1": "Project Manager",
        "2": "Developer",
        "3": "QA Engineer",
        "4": "Observer"
      },
      projects: {
        placeholder: 'Select project',
        select: 'Project'
      },
      team: {
        title: 'Team Members',
        subtitle: 'View current team composition, roles, and manage your access.',
        refreshBtn: 'Refresh',
        yourStatus: 'Your Status in the Project',
        youBadge: 'You',
        joinedAt: 'Joined',
        leaveBtn: 'Leave Project',
        teamMembers: 'Team Members ({count})',
        addedAt: 'Added',
        empty: {
          title: 'You are currently the only member of this project.',
          desc: 'Invite colleagues in the "Project Settings" tab.'
        },
        modal: {
          title: 'Leave the project team?',
          desc: 'This action is irreversible. You will completely lose access to the bug tracker, Kanban boards, and reports of this project.',
          cancelBtn: 'Cancel',
          confirmBtn: 'Yes, leave'
        },
        toast: {
          cannotLeave: {
            title: 'Action Impossible',
            desc: 'You are the Owner (Project Manager) of this project. Transfer management rights to another member in Settings before leaving the project.'
          },
          error: {
            title: 'Error',
            desc: 'Failed to leave the project.'
          }
        }
      },
      projectSettings: {
        "title": "Project Settings",
        "subtitle": "Manage project parameters, team access rights, and tracker configuration.",
        "accessDenied": "Access Restricted",
        "accessDeniedDesc": "Only the Owner (Project Manager) has access to this project's settings.",
        "tabs": {
          "general": "General Settings",
          "members": "Team Management",
          "danger": "Danger Zone"
        },
        "general": {
          "title": "Basic Information",
          "nameLabel": "Project Name",
          "prefixLabel": "Issue Prefix (Uppercase)",
          "descLabel": "Project Description",
          "descPlaceholder": "Brief description of team goals and objectives...",
          "saveBtn": "Save Changes"
        },
        "members": {
          "title": "Team Members ({count})",
          "creatorBadge": "Creator"
        },
        "danger": {
          "title": "Delete Project",
          "desc": "Proceed with extreme caution. This action will permanently delete the project along with all sprints, Kanban boards, issues (bugs), and attachment files. Data cannot be recovered.",
          "deleteBoxTitle": "Delete This Project",
          "projectNameLabel": "Project Name:",
          "deleteBtn": "Delete Project"
        },
        "modals": {
          "cancel": "Cancel",
          "kick": {
            "title": "Remove Team Member?",
            "desc": "Are you sure you want to remove {name} from the project? They will lose access to all issues and boards.",
            "confirm": "Yes, remove"
          },
          "delete": {
            "title": "Permanently Delete Project?",
            "desc": "This action cannot be undone. All sprints, issues, and attachments will be permanently erased.",
            "inputLabel": "To confirm, enter the project name:",
            "inputPlaceholder": "Enter project name",
            "confirm": "I understand, delete project"
          }
        },
        "notifications": {
          "nameMismatch": "Project name entered incorrectly",
          "errorTitle": "Error"
        }
      },
      profile: {
        title: 'Profile Settings',
        view: 'View',
        edit: 'Edit',
        save: 'Save Changes',
        cancel: 'Cancel',
        successUpdate: 'Profile updated successfully!',
        fields: {
          firstname: 'First Name',
          lastname: 'Last Name',
          patronymic: 'Middle Name',
          phone: 'Phone',
          statusMessage: 'Status (where are you?)',
          statusPlaceholder: 'e.g.: On vacation until 06/10',
          bio: 'About Me',
          position: 'Position',
          department: 'Department',
          skills: 'Skills',
          birthDate: 'Date of Birth',
          telegram: 'Telegram',
          github: 'GitHub'
        },
        validation: {
          required: 'This field is required'
        },
        error: {
          loading: 'Error loading data',
          title: 'Error',
          updateFailed: 'Failed to update profile'
        }
      },
      issues: {
        myActiveTitle: 'My Active Issues',
        myActiveSubtitle: 'List of all unresolved issues assigned to you across current projects.',
        total: 'Total: {count}',
        emptyTitle: 'No active issues',
        emptyDesc: 'Great job! You have no open or attention-requiring issues.',
        updatedAt: 'Updated: {date}',
        loading: 'Loading…',
        editBtn: 'Edit',
        blockedAlertTitle: 'Issue is blocked',
        blockedAlertDefaultDesc: 'No blocking reason provided.',
        description: 'Description',
        descriptionEmpty: 'No description provided.',
        stepsToReproduce: 'Steps to Reproduce',
        reproducibleStable: 'Reproduces stably:',
        yes: 'Yes',
        no: 'No',
        timeTracking: 'Estimation & Time Logging',
        timeEstimation: 'Estimated (h)',
        timeLogged: 'Logged (h)',
        timeRemaining: 'Remaining (h)',
        linkedIssues: 'Linked Issues',
        commentsTitle: 'Comments ({count})',
        commentsEmpty: 'No comments yet. Be the first!',
        writeComment: 'Write a comment',
        commentPlaceholder: 'Leave your comment here...',
        internalCommentLabel: 'Internal comment (team only)',
        sendCommentBtn: 'Send',
        issueDetails: 'Issue Details',
        fieldStatus: 'Status',
        fieldAssignee: 'Assignee',
        assigneeNone: 'Unassigned',
        fieldType: 'Type',
        fieldPriority: 'Priority',
        fieldSeverity: 'Severity',
        fieldReporter: 'Reporter',
        fieldComponent: 'Component',
        fieldVersion: 'Version',
        fieldFixVersion: 'Fix Version',
        fieldEnvironment: 'Environment',
        fieldTags: 'Tags',
        created: 'Created:',
        updated: 'Updated:',
        deadline: 'Deadline:',
        startedAt: 'Started at:',
        resolvedAt: 'Resolved at:',
        closedAt: 'Closed at:',
        newIssueTitle: 'New Issue',
        newIssueSubtitle: 'Provide a description and configure parameters for the new task',
        formTitleLabel: 'Issue Title',
        formTitlePlaceholder: 'e.g.: Develop auth module via JWT',
        formDescLabel: 'Description',
        formDescPlaceholder: 'Describe context, requirements or steps to reproduce...',
        formTypeLabel: 'Issue Type',
        formPriorityLabel: 'Priority',
        formAssigneePlaceholder: 'Unassigned',
        formTagsHint: 'Separate with commas',
        formTagsPlaceholder: 'frontend, bug, sprint-2',
        cancelBtn: 'Cancel',
        createBtn: 'Create Issue',
        severityLevels: {
          s4: 'S4 - Trivial',
          s3: 'S3 - Minor',
          s2: 'S2 - Major',
          s1: 'S1 - Blocker',
          unknown: 'Level {level}'
        },
        types: {
          bug: 'Bug',
          feature: 'Feature',
          improvement: 'Improvement',
          task: 'Task'
        },
        priorities: {
          low: 'Low',
          medium: 'Medium',
          high: 'High',
          critical: 'Critical'
        }
      }
    }
  }
}))