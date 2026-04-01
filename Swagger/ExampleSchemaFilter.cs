using System.Collections.Generic;
using digiEasy_backend.Models;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace digiEasy_backend.Swagger;

public sealed class ExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null)
        {
            return;
        }

        if (context.Type == typeof(RegisterRequest))
        {
            ApplyRows(schema, new (string key, string value, string desc)[]
            {
                ("firstName", "John", null),
                ("lastName", "Doe", null),
                ("email", "john.doe@example.com", null),
                ("password", "Abcd1234", "Min 8 chars; uppercase, lowercase, digit."),
                ("passwordConfirmation", "Abcd1234", "Must match password."),
                ("gender", "M", "M or F."),
                ("birthday", "2000-01-15", "ISO date; user must be 18+."),
                ("mobilePhoneNumber", "12345678", "Exactly 8 digits.")
            });
            return;
        }

        if (context.Type == typeof(UpdatePasswordRequest))
        {
            ApplyRows(schema, new (string, string, string)[]
            {
                ("email", "john.doe@example.com", null),
                ("currentPassword", "Abcd1234", "Current password."),
                ("newPassword", "Newpass123", "Same rules as registration."),
                ("newPasswordConfirmation", "Newpass123", "Must match newPassword.")
            });
            return;
        }

        if (context.Type == typeof(VerifyPasswordRequest))
        {
            ApplyRows(schema, new (string, string, string)[]
            {
                ("email", "john.doe@example.com", null),
                ("password", "Abcd1234", "Password to test.")
            });
            return;
        }

        if (context.Type == typeof(DeleteUserRequest))
        {
            ApplyRows(schema, new (string, string, string)[]
            {
                ("email", "john.doe@example.com", null),
                ("password", "Abcd1234", "Must match stored hash.")
            });
        }
    }

    private static void ApplyRows(OpenApiSchema schema, IReadOnlyList<(string key, string value, string desc)> rows)
    {
        var example = new OpenApiObject();
        foreach (var (key, value, desc) in rows)
        {
            if (TryGetProperty(schema, key, out var prop))
            {
                prop.Example = new OpenApiString(value);
                if (!string.IsNullOrEmpty(desc))
                {
                    prop.Description = string.IsNullOrEmpty(prop.Description) ? desc : $"{prop.Description} {desc}";
                }
            }

            example[key] = new OpenApiString(value);
        }

        schema.Example = example;
    }

    private static bool TryGetProperty(OpenApiSchema schema, string camelName, out OpenApiSchema prop)
    {
        prop = null;
        if (schema.Properties == null)
        {
            return false;
        }

        if (schema.Properties.TryGetValue(camelName, out prop))
        {
            return true;
        }

        var pascal = char.ToUpperInvariant(camelName[0]) + camelName.Substring(1);
        return schema.Properties.TryGetValue(pascal, out prop);
    }
}
