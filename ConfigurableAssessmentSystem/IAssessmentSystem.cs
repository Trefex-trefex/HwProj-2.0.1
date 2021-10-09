namespace ConfigurableAssessmentSystem
{
    public interface IAssessmentSystem
    {
        int CalculateAssessmentForTask(TaskModel task,
            SolutionModel[] solutions);
            /* общая оценка за весь курс, 1 моделька */
    }
}