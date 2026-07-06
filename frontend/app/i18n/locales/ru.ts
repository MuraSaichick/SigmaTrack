export default {
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
    register_subtitle: 'Заполните данные для создания учетной записи',
    validation: {
      error_title: 'Ошибка',
      password_mismatch: 'Пароли не совпадают!'
    }
  },
  fields: {
    login: 'Логин',
    password: 'Пароль',
    confirm_password: 'Подтвердите пароль',
    firstname: 'Имя',
    patronymic: 'Отчество (необязательно)',
    lastname: 'Фамилия',
    email: 'Электронная почта',
    phone: 'Номер телефона',
    patronymic_placeholder: 'При наличии'
  },
  menu: {
    dashboard: "Обзор",
    issues: "Задачи",
    kanban: "Канбан",
    sprints: "Спринты",
    team: "Команда",
    settings: "Настройки проекта",
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
    title: "Настройки проекта",
    subtitle: "Управление параметрами проекта, правами доступа команды и конфигурацией трекера.",
    accessDenied: "Доступ ограничен",
    accessDeniedDesc: "Только Владелец (Project Manager) имеет доступ к настройкам этого проекта.",
    tabs: {
      general: "Общие настройки",
      members: "Управление командой",
      danger: "Опасная зона"
    },
    general: {
      title: "Основная информация",
      nameLabel: "Название проекта",
      prefixLabel: "Префикс задач (Заглавные)",
      descLabel: "Описание проекта",
      descPlaceholder: "Краткое описание целей и задач команды...",
      saveBtn: "Сохранить изменения"
    },
    members: {
      title: "Участники команды ({count})",
      creatorBadge: "Создатель"
    },
    danger: {
      title: "Удаление проекта",
      desc: "Будьте предельно осторожны. Это действие полностью удалит проект вместе со всеми спринтами, канбан-досками, задачами (багами) и файлами вложений. Данные невозможно будет восстановить.",
      deleteBoxTitle: "Удалить этот проект",
      projectNameLabel: "Имя проекта:",
      deleteBtn: "Удалить проект"
    },
    modals: {
      cancel: "Отмена",
      kick: {
        title: "Исключить участника?",
        desc: "Вы действительно хотите удалить пользователя {name} из проекта? Он потеряет доступ ко всем задачам и доскам.",
        confirm: "Да, удалить"
      },
      delete: {
        title: "Удалить проект окончательно?",
        desc: "Это действие невозможно отменить. Все спринты, задачи и вложения будут стёрты навсегда.",
        inputLabel: "Для подтверждения введите имя проекта:",
        inputPlaceholder: "Введите название проекта",
        confirm: "Я понимаю, удалить проект"
      }
    },
    notifications: {
      nameMismatch: "Имя проекта введено неверно",
      errorTitle: "Ошибка"
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
}