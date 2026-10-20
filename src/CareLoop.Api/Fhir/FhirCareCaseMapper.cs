using System.Text.Json.Nodes;
using CareLoop.Domain.CareCases;
using CareLoop.Domain.Communications;
using CareLoop.Domain.FollowUps;

namespace CareLoop.Api.Fhir;

public static class FhirCareCaseMapper
{
    public static JsonObject ToBundle(CareCase careCase)
    {
        var entries = new JsonArray
        {
            CreateEntry(
            $"Patient/{careCase.Patient.Id}",
            CreatePatient(careCase)),
            CreateEntry(
            $"ServiceRequest/{careCase.DiagnosticOrder.Id}",
            CreateServiceRequest(careCase))
        };

        if (careCase.DiagnosticResult is not null)
        {
            entries.Add(CreateEntry(
                $"DiagnosticReport/{careCase.DiagnosticResult.Id}",
                CreateDiagnosticReport(careCase)));
        }

        foreach (var attempt in careCase.ContactAttempts)
        {
            entries.Add(CreateEntry(
                $"Communication/{attempt.Id}",
                CreateCommunication(careCase, attempt)));
        }

        foreach (var followUp in careCase.FollowUpActions)
        {
            entries.Add(CreateEntry(
                $"Task/{followUp.Id}",
                CreateTask(careCase, followUp)));
        }

        return new JsonObject
        {
            ["resourceType"] = "Bundle",
            ["type"] = "collection",
            ["timestamp"] = DateTimeOffset.UtcNow,
            ["entry"] = entries
        };
    }

    private static JsonObject CreatePatient(
        CareCase careCase)
    {
        return new JsonObject
        {
            ["resourceType"] = "Patient",
            ["id"] = careCase.Patient.Id.ToString(),
            ["identifier"] = new JsonArray
            {
                new JsonObject
                {
                    ["system"] =
                        "https://careloop.local/mrn",
                    ["value"] =
                        careCase.Patient.MedicalRecordNumber
                }
            },
            ["name"] = new JsonArray
            {
                new JsonObject
                {
                    ["family"] =
                        careCase.Patient.LastName,
                    ["given"] = new JsonArray
                    {
                        careCase.Patient.FirstName
                    }
                }
            },
            ["birthDate"] =
                careCase.Patient.DateOfBirth
                    .ToString("yyyy-MM-dd")
        };
    }

    private static JsonObject CreateServiceRequest(
        CareCase careCase)
    {
        return new JsonObject
        {
            ["resourceType"] = "ServiceRequest",
            ["id"] =
                careCase.DiagnosticOrder.Id.ToString(),

            ["status"] =
                careCase.DiagnosticResult is null
                    ? "active"
                    : "completed",

            ["intent"] = "order",

            ["code"] = new JsonObject
            {
                ["text"] =
                    careCase.DiagnosticOrder.TestName
            },

            ["subject"] = new JsonObject
            {
                ["reference"] =
                    $"Patient/{careCase.Patient.Id}"
            },

            ["authoredOn"] =
                careCase.DiagnosticOrder.OrderedAt
        };
    }

    private static JsonObject CreateDiagnosticReport(
        CareCase careCase)
    {
        var result = careCase.DiagnosticResult!;

        return new JsonObject
        {
            ["resourceType"] = "DiagnosticReport",
            ["id"] = result.Id.ToString(),

            ["status"] = "final",

            ["code"] = new JsonObject
            {
                ["text"] =
                    careCase.DiagnosticOrder.TestName
            },

            ["subject"] = new JsonObject
            {
                ["reference"] =
                    $"Patient/{careCase.Patient.Id}"
            },

            ["basedOn"] = new JsonArray
            {
                new JsonObject
                {
                    ["reference"] =
                        $"ServiceRequest/{careCase.DiagnosticOrder.Id}"
                }
            },

            ["issued"] = result.ReceivedAt,

            ["conclusion"] = result.Summary,

            ["extension"] = new JsonArray
            {
                new JsonObject
                {
                    ["url"] =
                        "https://careloop.local/fhir/StructureDefinition/result-priority",

                    ["valueCode"] =
                        result.Priority.ToString()
                }
            }
        };
    }

    private static JsonObject CreateCommunication(
        CareCase careCase,
        ContactAttempt attempt)
    {
        return new JsonObject
        {
            ["resourceType"] = "Communication",
            ["id"] = attempt.Id.ToString(),

            ["status"] =
                attempt.Successful
                    ? "completed"
                    : "not-done",

            ["subject"] = new JsonObject
            {
                ["reference"] =
                    $"Patient/{careCase.Patient.Id}"
            },

            ["basedOn"] = new JsonArray
            {
                new JsonObject
                {
                    ["reference"] =
                        $"ServiceRequest/{careCase.DiagnosticOrder.Id}"
                }
            },

            ["sent"] = attempt.AttemptedAt,

            ["medium"] = new JsonArray
            {
                new JsonObject
                {
                    ["text"] =
                        attempt.Method.ToString()
                }
            },

            ["payload"] = new JsonArray
            {
                new JsonObject
                {
                    ["contentString"] =
                        attempt.Notes
                }
            }
        };
    }

    private static JsonObject CreateTask(
        CareCase careCase,
        FollowUpAction followUp)
    {
        return new JsonObject
        {
            ["resourceType"] = "Task",
            ["id"] = followUp.Id.ToString(),

            ["status"] =
                MapTaskStatus(followUp.Status),

            ["intent"] = "order",

            ["description"] =
                followUp.Description,

            ["for"] = new JsonObject
            {
                ["reference"] =
                    $"Patient/{careCase.Patient.Id}"
            },

            ["basedOn"] = new JsonArray
            {
                new JsonObject
                {
                    ["reference"] =
                        $"ServiceRequest/{careCase.DiagnosticOrder.Id}"
                }
            },

            ["restriction"] = new JsonObject
            {
                ["period"] = new JsonObject
                {
                    ["end"] = followUp.DueAt
                }
            },

            ["executionPeriod"] =
                followUp.CompletedAt is null
                    ? null
                    : new JsonObject
                    {
                        ["end"] =
                            followUp.CompletedAt.Value
                    }
        };
    }

    private static string MapTaskStatus(
        FollowUpStatus status)
    {
        return status switch
        {
            FollowUpStatus.Pending =>
                "requested",

            FollowUpStatus.InProgress =>
                "in-progress",

            FollowUpStatus.Completed =>
                "completed",

            _ => "requested"
        };
    }

    private static JsonObject CreateEntry(
        string fullUrl,
        JsonObject resource)
    {
        return new JsonObject
        {
            ["fullUrl"] = fullUrl,
            ["resource"] = resource
        };
    }
}