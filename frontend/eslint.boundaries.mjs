import boundaries from "eslint-plugin-boundaries";

export const eslintBoundariesConfig = {
  plugins: {
    boundaries,
  },
  settings: {
    "import/resolver": {
      typescript: {
        alwaysTrue: true,
      },
    },

    "boundaries/elements": [
      {
        type: "root",
        pattern: "./app/*"
      },
      {
        type: "app",
        pattern: "./src/_app/*",
      },
      {
        type: "pages",
        pattern: "./src/_pages/*",
      },
      {
        type: "widgets",
        pattern: "./src/widgets/*",
      },
      {
        type: "features",
        pattern: "./src/features/*",
      },
      {
        type: "entities",
        pattern: "./src/entities/*",
      },
      {
        type: "shared",
        pattern: "./src/shared/*",
      },
    ],
  },
  rules: {
    "boundaries/element-types": [
      2,
      {
        default: "allow",
        rules: [
          {
            from: "shared",
            disallow: ["root", "app", "pages", "features", "widgets", "entities"],
            message: "Lower layer module (${file.type}) cannot import upper layer module (${dependency.type})",
          },
          {
            from: "entities",
            disallow: ["root", "app", "pages", "features", "widgets"],
            message: "Lower layer module (${file.type}) cannot import upper layer module (${dependency.type})",
          },
          {
            from: "features",
            disallow: ["root", "app", "pages", "widgets"],
            message: "Lower layer module (${file.type}) cannot import upper layer module (${dependency.type})",
          },
          {
            from: "widgets",
            disallow: ["root", "app", "pages"],
            message: "Lower layer module (${file.type}) cannot import upper layer module (${dependency.type})",
          },
          {
            from: "pages",
            disallow: ["root", "app"],
            message: "Lower layer module (${file.type}) cannot import upper layer module (${dependency.type})",
          },
          {
            from: "app",
            disallow: ["root"],
            message: "Lower layer module (${file.type}) cannot import upper layer module (${dependency.type})",
          }
        ],
      },
    ],
    "boundaries/entry-point": [
      2,
      {
        default: "disallow",
        message: "Module (${file.type}) must be imported using public API. Direct import from ${dependency.type} is disallowed",
        rules: [
          {
            target: ["shared"],
            allow: "**",
          },
          {
            target: ["pages", "features", "widgets", "entities"],
            allow: "index.(ts|tsx)",
          }
        ]
      }
    ]
  }
}