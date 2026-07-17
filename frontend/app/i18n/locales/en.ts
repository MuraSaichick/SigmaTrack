export default {
  auth: {
    project_name: 'SigmaTrack',
    tagline: 'Project management system',
    login_title: 'Sign In',
    register_title: 'Create Account',
    no_account: "Don't have an account?",
    have_account: 'Already have an account?',
    register_link: 'Sign Up',
    login_link: 'Sign In',
    login_btn: 'Sign In',
    register_btn: 'Create Account',
    register_subtitle: 'Fill in the details to create an account',
    validation: {
      error_title: 'Error',
      password_mismatch: 'Passwords do not match!'
    }
  },
  fields: {
    login: 'Login',
    password: 'Password',
    confirm_password: 'Confirm Password',
    firstname: 'First Name',
    patronymic: 'Patronymic (optional)',
    lastname: 'Last Name',
    email: 'Email Address',
    phone: 'Phone Number',
    patronymic_placeholder: 'If available'
  },
  menu: {
    dashboard: "Dashboard",
    issues: "Issues",
    kanban: "Kanban",
    sprints: "Sprints",
    team: "Team",
    project_settings: "Project Settings",
    my_tasks: 'My Tasks',
    board: 'Board',
    reports: 'Reports',
    my_profile: 'My Profile',
    account_settings: 'Settings',
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
  "settings": {
    "title": "Account Settings",
    "subtitle": "Manage your account security and privacy preferences.",
    "tabs": {
      "security": "Security",
      "privacy": "Privacy"
    },
    "security": {
      "title": "Credentials",
      "subtitle": "Change your email address and login password.",
      "emailLabel": "Email address",
      "emailDesc": "Used for notifications and logging in",
      "passwordChangeTitle": "Change password",
      "currentPassword": "Current password",
      "newPassword": "New password",
      "confirmPassword": "Confirm new password",
      "saveButton": "Save changes"
    },
    "privacy": {
      "title": "Privacy",
      "subtitle": "Control how your personal information is displayed to other team members.",
      "contactsLabel": "Who can see my contact info?",
      "contactsDesc": "Manage access to your Telegram, Skype, and phone number",
      "birthDateLabel": "Date of birth visibility",
      "birthDateDesc": "Choose who can view your date of birth",
      "invitationLabel": "Who can invite me to projects?",
      "invitationDesc": "Restrict incoming team and project invitations",
      "onlineStatusTitle": "Online status",
      "onlineStatusDesc": "Show when you are active on the platform.",
      "searchableTitle": "Profile discoverability",
      "searchableDesc": "Allow others to find your profile by username or last name.",
      "statusMsgTitle": "Status messages",
      "statusMsgDesc": "Display your current status (e.g., 'On vacation') on task cards.",
      "saveButton": "Apply settings",
      "contactVisibility": {
        "Everyone": "Everyone",
        "Team Only": "Team only",
        "Nobody": "Nobody"
      },
      "birthDateVisibility": {
        "FullDate": "Show full date",
        "MonthAndDayOnly": "Month and day only",
        "Hidden": "Hide date of birth"
      },
      "invitationRestriction": {
        "Everyone": "Everyone",
        "TeamOnly": "Team members only"
      }
    }
  },
  projectSettings: {
    title: "Project Settings",
    subtitle: "Manage project parameters, team access rights, and tracker configuration.",
    accessDenied: "Access Restricted",
    accessDeniedDesc: "Only the Owner (Project Manager) has access to this project's settings.",
    tabs: {
      general: "General Settings",
      members: "Team Management",
      danger: "Danger Zone"
    },
    general: {
      title: "Basic Information",
      nameLabel: "Project Name",
      prefixLabel: "Issue Prefix (Uppercase)",
      descLabel: "Project Description",
      descPlaceholder: "Brief description of team goals and objectives...",
      saveBtn: "Save Changes"
    },
    members: {
      title: "Team Members ({count})",
      creatorBadge: "Creator"
    },
    danger: {
      title: "Delete Project",
      desc: "Proceed with extreme caution. This action will permanently delete the project along with all sprints, Kanban boards, issues (bugs), and attachment files. Data cannot be recovered.",
      deleteBoxTitle: "Delete This Project",
      projectNameLabel: "Project Name:",
      deleteBtn: "Delete Project"
    },
    modals: {
      cancel: "Cancel",
      kick: {
        title: "Remove Team Member?",
        desc: "Are you sure you want to remove {name} from the project? They will lose access to all issues and boards.",
        confirm: "Yes, remove"
      },
      delete: {
        title: "Permanently Delete Project?",
        desc: "This action cannot be undone. All sprints, issues, and attachments will be permanently erased.",
        inputLabel: "To confirm, enter the project name:",
        inputPlaceholder: "Enter project name",
        confirm: "I understand, delete project"
      }
    },
    notifications: {
      nameMismatch: "Project name entered incorrectly",
      errorTitle: "Error"
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
    reproducibleStable: 'Reproduces stably',
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
    editPage: {
      title: 'Edit Issue',
      subtitle: 'Modify issue details',
      saveBtn: 'Save'
    },
    severityLevels: {
      critical: 'Critical',
      major: 'Major',
      minor: 'Minor',
      trivial: 'Trivial',
      enhancement: 'Enhancement',
      s1: 'Critical',
      s2: 'Major',
      s3: 'Minor',
      s4: 'Trivial',
      unknown: 'Level {level}'
    },
    types: {
      bug: 'Bug',
      feature: 'Feature',
      improvement: 'Improvement',
      task: 'Task'
    },
    priorities: {
      critical: 'Critical',
      high: 'High',
      medium: 'Medium',
      low: 'Low',
      trivial: 'Trivial'
    }
  },
  sprints: {
    title: 'Project Sprints',
    subtitle: 'Manage and plan iterations',
    createBtn: 'Create Sprint',
    loading: 'Loading…',
    errorTitle: 'Failed to load sprints',
    emptyTitle: 'No sprints found with the selected status.',
    noGoal: 'Goal is not specified',
    committedPoints: 'Committed: {points} SP',
    capacityLimit: 'Capacity: {limit} SP',
    statuses: {
      all: 'All',
      planning: 'Planning',
      active: 'Active',
      completed: 'Completed',
      cancelled: 'Cancelled'
    },
    statusLabels: {
      planning: 'Planning',
      active: 'Active',
      completed: 'Completed',
      cancelled: 'Cancelled',
      unknown: 'Unknown'
    },
    createPage: {
      title: 'Create Sprint',
      subtitle: 'Plan a new working iteration for the project',
      errorTitle: 'Creation failed',
      nameLabel: 'Sprint Name',
      namePlaceholder: 'e.g., Sprint 1: API Integration',
      goalLabel: 'Sprint Goal (optional)',
      goalPlaceholder: 'Describe the main business goal of this iteration...',
      startDateLabel: 'Start Date',
      endDateLabel: 'End Date',
      capacityLabel: 'Sprint Capacity (in Story Points)',
      capacityPlaceholder: '100',
      cancelBtn: 'Cancel',
      submitBtn: 'Create Sprint'
    },
    validation: {
      nameRequired: 'Sprint name is required.',
      nameMaxLength: 'Maximum length is 150 characters.',
      startDateRequired: 'Please specify start date.',
      endDateRequired: 'Please specify end date.',
      dateOrder: 'End date must be later than start date.',
      capacityMin: 'Capacity must be greater than 0.'
    }
  },
  sprintDetail: {
    headerPrefix: 'SPRINT',
    startSprintBtn: 'Start Sprint',
    completeSprintBtn: 'Complete Sprint',
    addIssuesBtn: 'Add Issues',
    cancelSprintBtn: 'Cancel Sprint',
    confirmCancelMessage: 'Are you sure you want to cancel this sprint? All incomplete tasks will return to the backlog.',
    goalTitle: 'Sprint Goal',
    goalEmpty: 'No goal has been formulated for this sprint.',
    issuesTitle: 'Sprint Issues ({count})',
    issuesEmpty: 'There are no issues in this sprint yet. Click "Add Issues" to fill the sprint.',
    sidebarTitle: 'Parameters & Capacity',
    startDateLabel: 'Start Date:',
    endDateLabel: 'End Date:',
    capacityLabel: 'Capacity',
    committedLabel: 'Committed',
    completedLabel: 'Completed',
    capacityProgressLabel: 'Capacity Load:',
    modal: {
      title: 'Add Issues to Sprint',
      empty: 'No available unassigned issues to add.',
      addBtn: 'Add Selected ({count})'
    }
  }
}