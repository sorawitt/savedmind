export default {
    extends: ['@commitlint/config-conventional'],
    rules: {
        'type-enum': [
            2,
            'always',
            [
                'feat',     // New feature
                'fix',      // Bug fix
                'docs',     // Documentation
                'style',    // Formatting (no code change)
                'refactor', // Code refactoring
                'perf',     // Performance improvement
                'test',     // Adding tests
                'chore',    // Build, config, etc.
                'ci',       // CI/CD changes
                'revert',   // Revert commit
            ],
        ],
        'subject-case': [2, 'always', 'lower-case'],
        'subject-empty': [2, 'never'],
        'type-empty': [2, 'never'],
    },
};
