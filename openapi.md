{
  "openapi": "3.0.4",
  "info": {
    "title": "Monetis.API",
    "version": "1.0"
  },
  "paths": {
    "/api/Accounts/{id}": {
      "get": {
        "tags": [
          "Accounts"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/AccountResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/AccountResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/AccountResponse"
                }
              }
            }
          }
        }
      },
      "put": {
        "tags": [
          "Accounts"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateAccountRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateAccountRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateAccountRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      },
      "delete": {
        "tags": [
          "Accounts"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/Accounts": {
      "get": {
        "tags": [
          "Accounts"
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/AccountResponse"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/AccountResponse"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/AccountResponse"
                  }
                }
              }
            }
          }
        }
      },
      "post": {
        "tags": [
          "Accounts"
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateAccountRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateAccountRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateAccountRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/AccountResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/AccountResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/AccountResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/Auth/login": {
      "post": {
        "tags": [
          "Auth"
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/LoginUserRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/LoginUserRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/LoginUserRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "string"
                }
              },
              "application/json": {
                "schema": {
                  "type": "string"
                }
              },
              "text/json": {
                "schema": {
                  "type": "string"
                }
              }
            }
          }
        }
      }
    },
    "/api/Cards/{id}": {
      "get": {
        "tags": [
          "Cards"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CardResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CardResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CardResponse"
                }
              }
            }
          }
        }
      },
      "put": {
        "tags": [
          "Cards"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateCardRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateCardRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateCardRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      },
      "delete": {
        "tags": [
          "Cards"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/Cards": {
      "get": {
        "tags": [
          "Cards"
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CardResponse"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CardResponse"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CardResponse"
                  }
                }
              }
            }
          }
        }
      },
      "post": {
        "tags": [
          "Cards"
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateCardRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateCardRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateCardRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CardResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CardResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CardResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/Categories/{id}": {
      "get": {
        "tags": [
          "Categories"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CategoryResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CategoryResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CategoryResponse"
                }
              }
            }
          }
        }
      },
      "put": {
        "tags": [
          "Categories"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateCategoryRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateCategoryRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateCategoryRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      },
      "delete": {
        "tags": [
          "Categories"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/Categories": {
      "get": {
        "tags": [
          "Categories"
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CategoryResponse"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CategoryResponse"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/CategoryResponse"
                  }
                }
              }
            }
          }
        }
      },
      "post": {
        "tags": [
          "Categories"
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateCategoryRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateCategoryRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateCategoryRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/CategoryResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/CategoryResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/CategoryResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/Expenses/{id}": {
      "get": {
        "tags": [
          "Expenses"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ExpenseResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ExpenseResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ExpenseResponse"
                }
              }
            }
          }
        }
      },
      "put": {
        "tags": [
          "Expenses"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateExpenseRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateExpenseRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateExpenseRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ExpenseResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ExpenseResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ExpenseResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/Expenses": {
      "get": {
        "tags": [
          "Expenses"
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/ExpenseResponse"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/ExpenseResponse"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/ExpenseResponse"
                  }
                }
              }
            }
          }
        }
      },
      "post": {
        "tags": [
          "Expenses"
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateExpenseRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateExpenseRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateExpenseRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ExpenseResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ExpenseResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ExpenseResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/Expenses/installments": {
      "post": {
        "tags": [
          "Expenses"
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateInstallmentRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateInstallmentRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateInstallmentRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/ExpenseResponse"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/ExpenseResponse"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/ExpenseResponse"
                  }
                }
              }
            }
          }
        }
      }
    },
    "/api/Expenses/{id}/pay": {
      "post": {
        "tags": [
          "Expenses"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/PayExpenseRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/PayExpenseRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/PayExpenseRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/ExpenseResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ExpenseResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/ExpenseResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/Expenses/process-overdue": {
      "post": {
        "tags": [
          "Expenses"
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/Incomes/{id}": {
      "get": {
        "tags": [
          "Incomes"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/IncomeResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/IncomeResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/IncomeResponse"
                }
              }
            }
          }
        }
      },
      "put": {
        "tags": [
          "Incomes"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateIncomeRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateIncomeRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateIncomeRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      },
      "delete": {
        "tags": [
          "Incomes"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/Incomes": {
      "get": {
        "tags": [
          "Incomes"
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/IncomeResponse"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/IncomeResponse"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/IncomeResponse"
                  }
                }
              }
            }
          }
        }
      },
      "post": {
        "tags": [
          "Incomes"
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateIncomeRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateIncomeRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateIncomeRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/IncomeResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/IncomeResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/IncomeResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/Subscriptions/{id}": {
      "get": {
        "tags": [
          "Subscriptions"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/SubscriptionResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/SubscriptionResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/SubscriptionResponse"
                }
              }
            }
          }
        }
      },
      "put": {
        "tags": [
          "Subscriptions"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateSubscriptionRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateSubscriptionRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateSubscriptionRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      },
      "delete": {
        "tags": [
          "Subscriptions"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/Subscriptions": {
      "get": {
        "tags": [
          "Subscriptions"
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/SubscriptionResponse"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/SubscriptionResponse"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/SubscriptionResponse"
                  }
                }
              }
            }
          }
        }
      },
      "post": {
        "tags": [
          "Subscriptions"
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateSubscriptionRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateSubscriptionRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateSubscriptionRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/SubscriptionResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/SubscriptionResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/SubscriptionResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/Transfers/{id}": {
      "get": {
        "tags": [
          "Transfers"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/TransferResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/TransferResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/TransferResponse"
                }
              }
            }
          }
        }
      },
      "put": {
        "tags": [
          "Transfers"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateTransferRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateTransferRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateTransferRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      },
      "delete": {
        "tags": [
          "Transfers"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/Transfers": {
      "get": {
        "tags": [
          "Transfers"
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/TransferResponse"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/TransferResponse"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/TransferResponse"
                  }
                }
              }
            }
          }
        }
      },
      "post": {
        "tags": [
          "Transfers"
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateTransferRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateTransferRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateTransferRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/TransferResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/TransferResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/TransferResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/Users/{id}": {
      "get": {
        "tags": [
          "Users"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/UserResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/UserResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/UserResponse"
                }
              }
            }
          }
        }
      },
      "put": {
        "tags": [
          "Users"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateUserRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateUserRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/UpdateUserRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      },
      "delete": {
        "tags": [
          "Users"
        ],
        "parameters": [
          {
            "name": "id",
            "in": "path",
            "required": true,
            "schema": {
              "type": "string",
              "format": "uuid"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "OK"
          }
        }
      }
    },
    "/api/Users": {
      "get": {
        "tags": [
          "Users"
        ],
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/UserResponse"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/UserResponse"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/UserResponse"
                  }
                }
              }
            }
          }
        }
      },
      "post": {
        "tags": [
          "Users"
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateUserRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/CreateUserRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/CreateUserRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "text/plain": {
                "schema": {
                  "$ref": "#/components/schemas/UserResponse"
                }
              },
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/UserResponse"
                }
              },
              "text/json": {
                "schema": {
                  "$ref": "#/components/schemas/UserResponse"
                }
              }
            }
          }
        }
      }
    }
  },
  "components": {
    "schemas": {
      "AccountResponse": {
        "type": "object",
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "name": {
            "type": "string",
            "nullable": true
          },
          "userId": {
            "type": "string",
            "format": "uuid"
          },
          "type": {
            "$ref": "#/components/schemas/AccountType"
          },
          "balance": {
            "type": "number",
            "format": "double"
          }
        },
        "additionalProperties": false
      },
      "AccountType": {
        "enum": [
          0,
          1,
          2
        ],
        "type": "integer",
        "format": "int32"
      },
      "CardResponse": {
        "type": "object",
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "name": {
            "type": "string",
            "nullable": true
          },
          "userId": {
            "type": "string",
            "format": "uuid"
          }
        },
        "additionalProperties": false
      },
      "CategoryResponse": {
        "type": "object",
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "name": {
            "type": "string",
            "nullable": true
          },
          "userId": {
            "type": "string",
            "format": "uuid"
          },
          "icon": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "CreateAccountRequest": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "nullable": true
          },
          "type": {
            "$ref": "#/components/schemas/AccountType"
          }
        },
        "additionalProperties": false
      },
      "CreateCardRequest": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "CreateCategoryRequest": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "nullable": true
          },
          "icon": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "CreateExpenseRequest": {
        "type": "object",
        "properties": {
          "accountId": {
            "type": "string",
            "format": "uuid"
          },
          "categoryId": {
            "type": "string",
            "format": "uuid"
          },
          "amount": {
            "type": "number",
            "format": "double"
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "dueDate": {
            "type": "string",
            "format": "date-time"
          },
          "paymentMethod": {
            "$ref": "#/components/schemas/PaymentMethod"
          },
          "creditCardId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "CreateIncomeRequest": {
        "type": "object",
        "properties": {
          "accountId": {
            "type": "string",
            "format": "uuid"
          },
          "categoryId": {
            "type": "string",
            "format": "uuid"
          },
          "amount": {
            "type": "number",
            "format": "double"
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "receivedAt": {
            "type": "string",
            "format": "date-time"
          }
        },
        "additionalProperties": false
      },
      "CreateInstallmentRequest": {
        "type": "object",
        "properties": {
          "accountId": {
            "type": "string",
            "format": "uuid"
          },
          "categoryId": {
            "type": "string",
            "format": "uuid"
          },
          "totalAmount": {
            "type": "number",
            "format": "double"
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "firstDueDate": {
            "type": "string",
            "format": "date-time"
          },
          "numberOfInstallments": {
            "type": "integer",
            "format": "int32"
          },
          "creditCardId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "CreateSubscriptionRequest": {
        "type": "object",
        "properties": {
          "accountId": {
            "type": "string",
            "format": "uuid"
          },
          "categoryId": {
            "type": "string",
            "format": "uuid"
          },
          "amount": {
            "type": "number",
            "format": "double"
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "frequency": {
            "$ref": "#/components/schemas/Frequency"
          },
          "nextDueDate": {
            "type": "string",
            "format": "date-time"
          },
          "paymentMethod": {
            "$ref": "#/components/schemas/PaymentMethod"
          }
        },
        "additionalProperties": false
      },
      "CreateTransferRequest": {
        "type": "object",
        "properties": {
          "accountId": {
            "type": "string",
            "format": "uuid"
          },
          "destinationAccountId": {
            "type": "string",
            "format": "uuid"
          },
          "amount": {
            "type": "number",
            "format": "double"
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "transferredAt": {
            "type": "string",
            "format": "date-time"
          }
        },
        "additionalProperties": false
      },
      "CreateUserRequest": {
        "type": "object",
        "properties": {
          "firstName": {
            "type": "string",
            "nullable": true
          },
          "lastName": {
            "type": "string",
            "nullable": true
          },
          "email": {
            "type": "string",
            "nullable": true
          },
          "password": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "ExpenseResponse": {
        "type": "object",
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "accountId": {
            "type": "string",
            "format": "uuid"
          },
          "categoryId": {
            "type": "string",
            "format": "uuid"
          },
          "status": {
            "$ref": "#/components/schemas/TransactionStatus"
          },
          "paidAt": {
            "type": "string",
            "format": "date-time",
            "nullable": true
          },
          "amount": {
            "type": "number",
            "format": "double"
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "dueDate": {
            "type": "string",
            "format": "date-time"
          },
          "paymentMethod": {
            "$ref": "#/components/schemas/PaymentMethod"
          },
          "installmentNumber": {
            "type": "integer",
            "format": "int32",
            "nullable": true
          },
          "totalInstallments": {
            "type": "integer",
            "format": "int32",
            "nullable": true
          },
          "installmentGroupId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          },
          "creditCardId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "Frequency": {
        "enum": [
          0,
          1,
          2,
          3,
          4,
          5,
          6
        ],
        "type": "integer",
        "format": "int32"
      },
      "IncomeResponse": {
        "type": "object",
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "accountId": {
            "type": "string",
            "format": "uuid"
          },
          "categoryId": {
            "type": "string",
            "format": "uuid"
          },
          "amount": {
            "type": "number",
            "format": "double"
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "receivedAt": {
            "type": "string",
            "format": "date-time"
          }
        },
        "additionalProperties": false
      },
      "LoginUserRequest": {
        "type": "object",
        "properties": {
          "email": {
            "type": "string",
            "nullable": true
          },
          "password": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "PayExpenseRequest": {
        "type": "object",
        "properties": {
          "paidAt": {
            "type": "string",
            "format": "date-time"
          },
          "accountId": {
            "type": "string",
            "format": "uuid",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "PaymentMethod": {
        "enum": [
          0,
          1,
          2,
          3,
          4
        ],
        "type": "integer",
        "format": "int32"
      },
      "SubscriptionResponse": {
        "type": "object",
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "amount": {
            "type": "number",
            "format": "double"
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "frequency": {
            "$ref": "#/components/schemas/Frequency"
          },
          "nextDueDate": {
            "type": "string",
            "format": "date-time"
          },
          "isActive": {
            "type": "boolean"
          }
        },
        "additionalProperties": false
      },
      "TransactionStatus": {
        "enum": [
          0,
          1,
          2,
          3
        ],
        "type": "integer",
        "format": "int32"
      },
      "TransferResponse": {
        "type": "object",
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "accountId": {
            "type": "string",
            "format": "uuid"
          },
          "destinationAccountId": {
            "type": "string",
            "format": "uuid"
          },
          "amount": {
            "type": "number",
            "format": "double"
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "transferredAt": {
            "type": "string",
            "format": "date-time"
          }
        },
        "additionalProperties": false
      },
      "UpdateAccountRequest": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "UpdateCardRequest": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "UpdateCategoryRequest": {
        "type": "object",
        "properties": {
          "name": {
            "type": "string",
            "nullable": true
          },
          "icon": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "UpdateExpenseRequest": {
        "type": "object",
        "properties": {
          "categoryId": {
            "type": "string",
            "format": "uuid"
          },
          "amount": {
            "type": "number",
            "format": "double"
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "dueDate": {
            "type": "string",
            "format": "date-time"
          },
          "isUpdatingGroup": {
            "type": "boolean",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "UpdateIncomeRequest": {
        "type": "object",
        "properties": {
          "categoryId": {
            "type": "string",
            "format": "uuid"
          },
          "amount": {
            "type": "number",
            "format": "double"
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "receivedAt": {
            "type": "string",
            "format": "date-time"
          }
        },
        "additionalProperties": false
      },
      "UpdateSubscriptionRequest": {
        "type": "object",
        "properties": {
          "amount": {
            "type": "number",
            "format": "double"
          },
          "description": {
            "type": "string",
            "nullable": true
          },
          "frequency": {
            "$ref": "#/components/schemas/Frequency"
          },
          "nextDueDate": {
            "type": "string",
            "format": "date-time"
          },
          "isActive": {
            "type": "boolean"
          }
        },
        "additionalProperties": false
      },
      "UpdateTransferRequest": {
        "type": "object",
        "properties": {
          "amount": {
            "type": "number",
            "format": "double"
          },
          "description": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "UpdateUserRequest": {
        "type": "object",
        "properties": {
          "firstName": {
            "type": "string",
            "nullable": true
          },
          "lastName": {
            "type": "string",
            "nullable": true
          },
          "email": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "UserResponse": {
        "type": "object",
        "properties": {
          "id": {
            "type": "string",
            "format": "uuid"
          },
          "firstName": {
            "type": "string",
            "nullable": true
          },
          "lastName": {
            "type": "string",
            "nullable": true
          },
          "email": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      }
    }
  },
  "tags": [
    {
      "name": "Accounts"
    },
    {
      "name": "Auth"
    },
    {
      "name": "Cards"
    },
    {
      "name": "Categories"
    },
    {
      "name": "Expenses"
    },
    {
      "name": "Incomes"
    },
    {
      "name": "Subscriptions"
    },
    {
      "name": "Transfers"
    },
    {
      "name": "Users"
    }
  ]
}
