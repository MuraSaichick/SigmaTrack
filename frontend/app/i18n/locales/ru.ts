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
    project_settings: "Настройки проекта",
    my_tasks: 'Мои задачи',
    board: 'Канбан-доска',
    reports: 'Аналитика',
    my_profile: 'Мой профиль',
    account_settings: 'Настройки',
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
    },
  },
  settings: {
    title: "Настройки аккаунта",
    subtitle: "Управление безопасностью учетной записи и параметрами приватности в системе.",
    tabs: {
      security: "Безопасность",
      privacy: "Приватность"
    },
    security: {
      title: "Учетные данные",
      subtitle: "Изменение почтового ящика и пароля для входа.",
      emailLabel: "Электронная почта",
      emailDesc: "Используется для уведомлений и входа",
      passwordChangeTitle: "Смена пароля",
      currentPassword: "Текущий пароль",
      newPassword: "Новый пароль",
      confirmPassword: "Подтвердите новый пароль",
      saveButton: "Сохранить изменения"
    },
    privacy: {
      title: "Конфиденциальность",
      subtitle: "Настройки отображения ваших данных для других участников команд.",
      contactsLabel: "Кто видит мои контакты?",
      contactsDesc: "Управляет доступом к вашему Telegram, Skype и телефону",
      birthDateLabel: "Отображение даты рождения",
      birthDateDesc: "Укажите, кто видит вашу дату рождения",
      invitationLabel: "Кто может приглашать меня в проекты?",
      invitationDesc: "Ограничение входящих приглашений в команды",
      onlineStatusTitle: "Статус «В сети»",
      onlineStatusDesc: "Показывать, когда вы активны на платформе.",
      searchableTitle: "Участие в поиске",
      searchableDesc: "Разрешить находить ваш профиль по логину или фамилии.",
      statusMsgTitle: "Статусные сообщения",
      statusMsgDesc: "Отображать ваш текущий статус (например, «В отпуске») в карточке задач.",
      saveButton: "Применить настройки",
      contactVisibility: {
        Everyone: "Все пользователи",
        TeamOnly: "Только команда",
        Nobody: "Никто"
      },
      birthDateVisibility: {
        FullDate: "Показывать полностью",
        MonthAndDayOnly: "Только день и месяц",
        Hidden: "Скрыть дату рождения"
      },
      invitationRestriction: {
        Everyone: "Все пользователи",
        TeamOnly: "Только участники команд"
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
    reproducibleStable: 'Воспроизводится стабильно',
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
    fieldSeverity: 'Серьезность (Severity)',
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
    editPage: {
      title: 'Редактирование задачи',
      subtitle: 'Изменение деталей задачи',
      saveBtn: 'Сохранить'
    },
    severityLevels: {
      critical: 'Critical (Критический)',
      major: 'Major (Высокий)',
      minor: 'Minor (Средний)',
      trivial: 'Trivial (Незначительный)',
      enhancement: 'Enhancement (Улучшение)',
      s1: 'Critical (Критический)',
      s2: 'Major (Высокий)',
      s3: 'Minor (Средний)',
      s4: 'Trivial (Незначительный)',
      unknown: 'Уровень {level}'
    },
    types: {
      bug: 'Баг',
      feature: 'Фича',
      improvement: 'Улучшение',
      task: 'Задача'
    },
    priorities: {
      critical: 'Критический',
      high: 'Высокий',
      medium: 'Средний',
      low: 'Низкий',
      trivial: 'Trivial (Минорный)'
    }
  },
  sprints: {
    title: 'Спринты проекта',
    subtitle: 'Управление и планирование итераций',
    createBtn: 'Создать спринт',
    loading: 'Загрузка…',
    errorTitle: 'Ошибка загрузки',
    emptyTitle: 'Спринты с выбранным статусом не найдены.',
    noGoal: 'Цель не указана',
    committedPoints: 'Запланировано: {points} SP',
    capacityLimit: 'Лимит: {limit} SP',
    statuses: {
      all: 'Все',
      planning: 'Планируются',
      active: 'Активные',
      completed: 'Завершенные',
      cancelled: 'Отмененные'
    },
    statusLabels: {
      planning: 'Планирование',
      active: 'Активен',
      completed: 'Завершен',
      cancelled: 'Отменен',
      unknown: 'Неизвестно'
    },
    createPage: {
      title: 'Создание спринта',
      subtitle: 'Запланируйте новую рабочую итерацию проекта',
      errorTitle: 'Ошибка создания',
      nameLabel: 'Название спринта',
      namePlaceholder: 'Например, Спринт 1: Интеграция API',
      goalLabel: 'Цель спринта (необязательно)',
      goalPlaceholder: 'Опишите основную бизнес-цель данной итерации...',
      startDateLabel: 'Дата начала',
      endDateLabel: 'Дата окончания',
      capacityLabel: 'Емкость спринта (Capacity в Story Points)',
      capacityPlaceholder: '100',
      cancelBtn: 'Отмена',
      submitBtn: 'Создать спринт'
    },
    validation: {
      nameRequired: 'Название спринта обязательно.',
      nameMaxLength: 'Максимальная длина 150 символов.',
      startDateRequired: 'Укажите дату начала.',
      endDateRequired: 'Укажите дату окончания.',
      dateOrder: 'Дата окончания должна быть позже даты начала.',
      capacityMin: 'Емкость должна быть больше 0.'
    }
  },
  sprintDetail: {
    headerPrefix: 'СПРИНТ',
    startSprintBtn: 'Запустить спринт',
    completeSprintBtn: 'Завершить спринт',
    addIssuesBtn: 'Добавить задачи',
    cancelSprintBtn: 'Отменить спринт',
    confirmCancelMessage: 'Вы уверены, что хотите отменить этот спринт? Все незавершенные задачи вернутся в бэклог.',
    goalTitle: 'Цель спринта',
    goalEmpty: 'Цель для этого спринта не сформулирована.',
    issuesTitle: 'Задачи спринта ({count})',
    issuesEmpty: 'В этом спринте пока нет задач. Нажмите кнопку «Добавить задачи», чтобы наполнить спринт.',
    sidebarTitle: 'Параметры и Емкость',
    startDateLabel: 'Дата начала:',
    endDateLabel: 'Дата окончания:',
    capacityLabel: 'Емкость',
    committedLabel: 'Взято',
    completedLabel: 'Закрыто',
    capacityProgressLabel: 'Загрузка емкости:',
    modal: {
      title: 'Добавление задач в спринт',
      empty: 'Нет доступных свободных задач для добавления.',
      addBtn: 'Добавить выбранные ({count})'
    }
  }
}