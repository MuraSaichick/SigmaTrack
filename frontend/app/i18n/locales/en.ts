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
    settings: "Project Settings",
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