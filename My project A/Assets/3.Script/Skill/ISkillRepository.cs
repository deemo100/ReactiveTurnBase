using System.Collections.Generic;

public interface ISkillRepository
{
    IReadOnlyList<SkillData> GetAllSkills();
    SkillData               GetSkillById(int id);
}