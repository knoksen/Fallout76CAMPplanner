namespace FO76CampPlanner;

// ─── CAMP Score Engine ────────────────────────────────────────────────────────

internal static class CampScoreEngine
{
    /// <summary>
    /// Computes a CAMP Score for the given project across five categories.
    /// All math is pure — no side effects.
    /// </summary>
    public static CampScore Calculate(PlannerProject project)
    {
        var items = project.Items;
        var budget = Math.Max(1, project.BudgetLimit);
        var placedBudget = items.Sum(x =>
            Catalog.ById.TryGetValue(x.DefinitionId, out var d) ? d.BudgetCost : 0);
        var totalBudget = placedBudget + project.StoredBudget;

        // Budget efficiency: reward spending 60–95 % of limit
        var pct = (double)totalBudget / budget;
        var budgetScore = pct switch
        {
            >= 0.90 and < 1.0 => 250,
            >= 0.70 and < 0.90 => 200,
            >= 0.60 and < 0.70 => 150,
            >= 0.30 and < 0.60 => 80,
            >= 1.0             => 120, // over budget — penalised
            _                  => 30
        };

        // Defense coverage: turrets + trap zones
        var turretCount = items.Count(x =>
            Catalog.ById.TryGetValue(x.DefinitionId, out var d) && d.Layer == LayerType.Defense);
        var trapZoneCount = project.TrapZones.Count;
        var defenseRaw = Math.Min(8, turretCount) * 20 + Math.Min(6, trapZoneCount) * 15;
        var defenseScore = Math.Min(250, defenseRaw);

        // Power network: power items vs total items
        var totalItems = items.Count;
        if (totalItems == 0)
        {
            return new CampScore(0, 0, 0, 0, 0);
        }

        var powerItems = items.Count(x =>
            Catalog.ById.TryGetValue(x.DefinitionId, out var d) && d.Layer == LayerType.Power);
        var powerRatio = (double)powerItems / totalItems;
        var powerScore = powerRatio switch
        {
            >= 0.15 => 200,
            >= 0.08 => 140,
            >= 0.03 => 80,
            _       => 20
        };

        // Visitor flow: ingress + egress + checkpoints
        var ingressCount = project.VisitorMarkers.Count(m => m.Type == VisitorMarkerType.Ingress);
        var egressCount = project.VisitorMarkers.Count(m => m.Type == VisitorMarkerType.Egress);
        var checkpointCount = project.VisitorMarkers.Count(m => m.Type == VisitorMarkerType.Checkpoint);
        var flowScore = 0;
        if (ingressCount > 0) flowScore += 80;
        if (egressCount > 0) flowScore += 60;
        flowScore += Math.Min(3, checkpointCount) * 20;

        // Aesthetics: decor/vendor/display ratio
        var aestheticItems = items.Count(x =>
            Catalog.ById.TryGetValue(x.DefinitionId, out var d) &&
            d.Layer is LayerType.Aesthetic or LayerType.Commerce);
        var aestheticRatio = (double)aestheticItems / totalItems;
        var aestheticScore = aestheticRatio switch
        {
            >= 0.20 => 100,
            >= 0.10 => 70,
            >= 0.05 => 40,
            _       => 10
        };

        return new CampScore(budgetScore, defenseScore, powerScore, flowScore, aestheticScore);
    }
}

// ─── Challenge Library ────────────────────────────────────────────────────────

internal static class ChallengeLibrary
{
    public static readonly IReadOnlyList<ChallengeDefinition> All =
        new List<ChallengeDefinition>
        {
            // Free tier challenges (3)
            new()
            {
                Id = "first-base",
                Name = "First Base",
                Description = "Get your CAMP running with the basics.",
                RequiredTier = PremiumTier.Free,
                Constraints = new[]
                {
                    new ChallengeConstraint
                    {
                        Description = "Place at least 5 foundation cells",
                        Predicate = p => p.Items.Count(x => IsDefinitionId(x, "foundation")) >= 5
                    },
                    new ChallengeConstraint
                    {
                        Description = "Place at least 1 workbench",
                        Predicate = p => p.Items.Any(x => IsDefinitionId(x, "workbench"))
                    },
                    new ChallengeConstraint
                    {
                        Description = "Place at least 1 power source",
                        Predicate = p => p.Items.Any(x =>
                            Catalog.ById.TryGetValue(x.DefinitionId, out var d) && d.Layer == LayerType.Power)
                    }
                }
            },
            new()
            {
                Id = "open-for-business",
                Name = "Open for Business",
                Description = "Build a vendor-ready trading post.",
                RequiredTier = PremiumTier.Free,
                Constraints = new[]
                {
                    new ChallengeConstraint
                    {
                        Description = "Place 3 or more vendor stalls",
                        Predicate = p => p.Items.Count(x => IsDefinitionId(x, "vendor")) >= 3
                    },
                    new ChallengeConstraint
                    {
                        Description = "Add an ingress marker for visitors",
                        Predicate = p => p.VisitorMarkers.Any(m => m.Type == VisitorMarkerType.Ingress)
                    },
                    new ChallengeConstraint
                    {
                        Description = "Keep defense budget below 25 % of total",
                        Predicate = p =>
                        {
                            var total = p.Items.Sum(x => Catalog.ById.TryGetValue(x.DefinitionId, out var d) ? d.BudgetCost : 0);
                            var defense = p.Items.Sum(x =>
                                Catalog.ById.TryGetValue(x.DefinitionId, out var d) && d.Layer == LayerType.Defense
                                    ? d.BudgetCost : 0);
                            return total > 0 && (double)defense / total < 0.25;
                        }
                    }
                }
            },
            new()
            {
                Id = "light-it-up",
                Name = "Light It Up",
                Description = "Ensure your CAMP is well-lit and visible.",
                RequiredTier = PremiumTier.Free,
                Constraints = new[]
                {
                    new ChallengeConstraint
                    {
                        Description = "Place at least 4 light sources",
                        Predicate = p => p.Items.Count(x => IsDefinitionId(x, "light")) >= 4
                    },
                    new ChallengeConstraint
                    {
                        Description = "Have a power source connected",
                        Predicate = p => p.Items.Any(x =>
                            Catalog.ById.TryGetValue(x.DefinitionId, out var d) && d.Layer == LayerType.Power)
                    }
                }
            },

            // Pro tier challenges
            new()
            {
                Id = "nuke-camp-ready",
                Name = "Nuke Camp Ready",
                Description = "Build a fast-launch tactical CAMP.",
                RequiredTier = PremiumTier.Pro,
                Constraints = new[]
                {
                    new ChallengeConstraint
                    {
                        Description = "Use Nuke Surface CAMP preset",
                        Predicate = p => p.PresetId == "nuke-camp"
                    },
                    new ChallengeConstraint
                    {
                        Description = "Place 2 or more workbenches",
                        Predicate = p => p.Items.Count(x => IsDefinitionId(x, "workbench")) >= 2
                    },
                    new ChallengeConstraint
                    {
                        Description = "Place 6 or more turrets",
                        Predicate = p => p.Items.Count(x => IsDefinitionId(x, "turret")) >= 6
                    },
                    new ChallengeConstraint
                    {
                        Description = "No vendor stalls (combat CAMP only)",
                        Predicate = p => p.Items.All(x => !IsDefinitionId(x, "vendor"))
                    }
                }
            },
            new()
            {
                Id = "stealth-camp",
                Name = "Ghost Camp",
                Description = "Maximum traps, zero turrets — pure misdirection.",
                RequiredTier = PremiumTier.Pro,
                Constraints = new[]
                {
                    new ChallengeConstraint
                    {
                        Description = "No turrets placed",
                        Predicate = p => p.Items.All(x => !IsDefinitionId(x, "turret"))
                    },
                    new ChallengeConstraint
                    {
                        Description = "At least 4 trap zones defined",
                        Predicate = p => p.TrapZones.Count >= 4
                    },
                    new ChallengeConstraint
                    {
                        Description = "Budget used below 60 %",
                        Predicate = p =>
                        {
                            var used = p.Items.Sum(x => Catalog.ById.TryGetValue(x.DefinitionId, out var d) ? d.BudgetCost : 0)
                                       + p.StoredBudget;
                            return (double)used / Math.Max(1, p.BudgetLimit) < 0.60;
                        }
                    }
                }
            },
            new()
            {
                Id = "fortress",
                Name = "Fortress",
                Description = "Total defense saturation.",
                RequiredTier = PremiumTier.Pro,
                Constraints = new[]
                {
                    new ChallengeConstraint
                    {
                        Description = "Place 10 or more turrets",
                        Predicate = p => p.Items.Count(x => IsDefinitionId(x, "turret")) >= 10
                    },
                    new ChallengeConstraint
                    {
                        Description = "Have ingress and egress markers",
                        Predicate = p =>
                            p.VisitorMarkers.Any(m => m.Type == VisitorMarkerType.Ingress) &&
                            p.VisitorMarkers.Any(m => m.Type == VisitorMarkerType.Egress)
                    },
                    new ChallengeConstraint
                    {
                        Description = "At least 2 High or Critical trap zones",
                        Predicate = p => p.TrapZones.Count(z =>
                            z.Severity is TrapZoneSeverity.High or TrapZoneSeverity.Critical) >= 2
                    }
                }
            },

            // Vault-Tec Elite challenges
            new()
            {
                Id = "budget-perfectionist",
                Name = "Budget Perfectionist",
                Description = "Spend between 90–98 % of budget limit — not a cap over.",
                RequiredTier = PremiumTier.VaultTecElite,
                Constraints = new[]
                {
                    new ChallengeConstraint
                    {
                        Description = "Total budget usage between 90 % and 98 %",
                        Predicate = p =>
                        {
                            var used = p.Items.Sum(x => Catalog.ById.TryGetValue(x.DefinitionId, out var d) ? d.BudgetCost : 0)
                                       + p.StoredBudget;
                            var ratio = (double)used / Math.Max(1, p.BudgetLimit);
                            return ratio is >= 0.90 and < 0.99;
                        }
                    },
                    new ChallengeConstraint
                    {
                        Description = "Place items across at least 4 different layers",
                        Predicate = p =>
                        {
                            var layers = p.Items
                                .Select(x => Catalog.ById.TryGetValue(x.DefinitionId, out var d) ? (LayerType?)d.Layer : null)
                                .Where(l => l.HasValue)
                                .Select(l => l!.Value)
                                .Distinct()
                                .Count();
                            return layers >= 4;
                        }
                    }
                }
            },
            new()
            {
                Id = "showcase-ready",
                Name = "Showcase Ready",
                Description = "A CAMP worthy of being featured.",
                RequiredTier = PremiumTier.VaultTecElite,
                Constraints = new[]
                {
                    new ChallengeConstraint
                    {
                        Description = "Achieve a CAMP Score of A or S",
                        Predicate = p =>
                        {
                            var score = CampScoreEngine.Calculate(p);
                            return score.Grade is "A" or "S";
                        }
                    },
                    new ChallengeConstraint
                    {
                        Description = "Have a full visitor route (ingress + checkpoint + egress)",
                        Predicate = p =>
                            p.VisitorMarkers.Any(m => m.Type == VisitorMarkerType.Ingress) &&
                            p.VisitorMarkers.Any(m => m.Type == VisitorMarkerType.Checkpoint) &&
                            p.VisitorMarkers.Any(m => m.Type == VisitorMarkerType.Egress)
                    },
                    new ChallengeConstraint
                    {
                        Description = "Aesthetic + Commerce items make up at least 20 % of total",
                        Predicate = p =>
                        {
                            var total = p.Items.Count;
                            if (total == 0) return false;
                            var aesthetic = p.Items.Count(x =>
                                Catalog.ById.TryGetValue(x.DefinitionId, out var d) &&
                                d.Layer is LayerType.Aesthetic or LayerType.Commerce);
                            return (double)aesthetic / total >= 0.20;
                        }
                    }
                }
            }
        };

    private static bool IsDefinitionId(PlacedItem item, string id)
        => string.Equals(item.DefinitionId, id, StringComparison.Ordinal);
}

// ─── Challenge Engine ─────────────────────────────────────────────────────────

internal static class ChallengeEngine
{
    public static IReadOnlyList<ChallengeConstraintResult> Evaluate(
        ChallengeDefinition challenge,
        PlannerProject project)
    {
        return challenge.Constraints
            .Select(c => new ChallengeConstraintResult
            {
                Description = c.Description,
                IsMet = c.Predicate(project)
            })
            .ToList();
    }

    public static bool IsComplete(ChallengeDefinition challenge, PlannerProject project)
        => Evaluate(challenge, project).All(r => r.IsMet);
}

// ─── Achievement Library ──────────────────────────────────────────────────────

internal static class AchievementLibrary
{
    public static readonly IReadOnlyList<AchievementDefinition> All =
        new List<AchievementDefinition>
        {
            new() { Id = AchievementId.FirstBlueprint,   Name = "First Blueprint",   Description = "Save your first project.",                    Icon = "◈", RequiredTier = PremiumTier.Free },
            new() { Id = AchievementId.BudgetMaster,     Name = "Budget Master",     Description = "Complete a build within 5 % of budget cap.",  Icon = "◎", RequiredTier = PremiumTier.Free },
            new() { Id = AchievementId.Fortress,         Name = "Fortress",          Description = "Place 10 or more turrets in one layout.",      Icon = "⬡", RequiredTier = PremiumTier.Free },
            new() { Id = AchievementId.OpenForBusiness,  Name = "Open for Business", Description = "Add 5 or more vendor items.",                  Icon = "◆", RequiredTier = PremiumTier.Free },
            new() { Id = AchievementId.VisitorReady,     Name = "Visitor Ready",     Description = "Define a full ingress–checkpoint–egress route.",Icon = "▷", RequiredTier = PremiumTier.Free },
            new() { Id = AchievementId.TrapArchitect,    Name = "Trap Architect",    Description = "Create 5 or more trap zones.",                 Icon = "⚠", RequiredTier = PremiumTier.Pro },
            new() { Id = AchievementId.PowerGrid,        Name = "Power Grid",        Description = "Power items make up 15 %+ of your build.",     Icon = "⚡", RequiredTier = PremiumTier.Pro },
            new() { Id = AchievementId.PlannerElite,     Name = "Planner Elite",     Description = "Achieve an S-rank CAMP Score.",                Icon = "★", RequiredTier = PremiumTier.VaultTecElite }
        };
}

// ─── Achievement Engine ───────────────────────────────────────────────────────

internal static class AchievementEngine
{
    /// <summary>
    /// Evaluates which achievements the current project state unlocks and
    /// persists newly unlocked ones via PremiumStore.
    /// Returns the set of newly unlocked achievement IDs.
    /// </summary>
    public static IReadOnlyList<AchievementId> EvaluateAndPersist(PlannerProject project)
    {
        var newlyUnlocked = new List<AchievementId>();

        foreach (var def in AchievementLibrary.All)
        {
            if (PremiumStore.IsUnlocked(def.Id)) continue;
            if (!PremiumStore.State.Has(def.RequiredTier)) continue;

            if (IsEarned(def.Id, project))
            {
                PremiumStore.UnlockAchievement(def.Id);
                newlyUnlocked.Add(def.Id);
            }
        }

        return newlyUnlocked;
    }

    private static bool IsEarned(AchievementId id, PlannerProject project)
    {
        var items = project.Items;
        return id switch
        {
            AchievementId.FirstBlueprint =>
                PremiumStore.Stats.TotalSaves >= 1,

            AchievementId.BudgetMaster =>
                BudgetEfficiencyPct(project) is >= 0.95 and < 1.0,

            AchievementId.Fortress =>
                items.Count(x => string.Equals(x.DefinitionId, "turret", StringComparison.Ordinal)) >= 10,

            AchievementId.OpenForBusiness =>
                items.Count(x => string.Equals(x.DefinitionId, "vendor", StringComparison.Ordinal)) >= 5,

            AchievementId.VisitorReady =>
                project.VisitorMarkers.Any(m => m.Type == VisitorMarkerType.Ingress) &&
                project.VisitorMarkers.Any(m => m.Type == VisitorMarkerType.Checkpoint) &&
                project.VisitorMarkers.Any(m => m.Type == VisitorMarkerType.Egress),

            AchievementId.TrapArchitect =>
                project.TrapZones.Count >= 5,

            AchievementId.PowerGrid =>
                PowerRatio(project) >= 0.15,

            AchievementId.PlannerElite =>
                CampScoreEngine.Calculate(project).Grade == "S",

            _ => false
        };
    }

    private static double BudgetEfficiencyPct(PlannerProject project)
    {
        var used = project.Items.Sum(x => Catalog.ById.TryGetValue(x.DefinitionId, out var d) ? d.BudgetCost : 0)
                   + project.StoredBudget;
        return (double)used / Math.Max(1, project.BudgetLimit);
    }

    private static double PowerRatio(PlannerProject project)
    {
        var total = project.Items.Count;
        if (total == 0) return 0;
        var power = project.Items.Count(x =>
            Catalog.ById.TryGetValue(x.DefinitionId, out var d) && d.Layer == LayerType.Power);
        return (double)power / total;
    }
}
