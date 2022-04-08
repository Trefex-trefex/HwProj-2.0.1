import * as React from "react";
import {RouteComponentProps} from "react-router-dom";
import {CourseViewModel, HomeworkTaskViewModel} from "../../api/";
import Typography from "@material-ui/core/Typography";
import Task from "../Tasks/Task";
import ApiSingleton from "../../api/ApiSingleton";
import {FC, useEffect, useState} from "react";
import {Grid, Link} from "@material-ui/core";
import GroupTaskSolutions from "./GroupTaskSolutions";

interface IGroupTaskSolutionsPageProps {
    groupId: string;
    taskId: string;
    userId: string;
}

interface IGroupTaskSolutionsPageState {
    task: HomeworkTaskViewModel;
    isLoaded: boolean;
    course: CourseViewModel;
}

const GroupTaskSolutionsPage: FC<RouteComponentProps<IGroupTaskSolutionsPageProps>> = (props) => {

    const [studentSolutions, setStudentSolutions] = useState<IGroupTaskSolutionsPageState>({
        task: {},
        isLoaded: false,
        course: {},
    })

    const {isLoaded} = studentSolutions
    const userId = ApiSingleton.authService.isLoggedIn()
        ? ApiSingleton.authService.getUserId()
        : undefined

    useEffect(() => {
        getStudentSolutions()
    }, [])

    const getStudentSolutions = async () => {
        console.log(props.match.params.userId)
        console.log(props.match.params.taskId)
        console.log(props.match.params.groupId)
        const task = await ApiSingleton.tasksApi.apiTasksGetByTaskIdGet(+props.match.params.taskId)
        const homework = await ApiSingleton.homeworksApi.apiHomeworksGetByHomeworkIdGet(task.homeworkId!)
        const course = await ApiSingleton.coursesApi.apiCoursesByCourseIdGet(homework.courseId!)

        setStudentSolutions({
            task: task,
            isLoaded: true,
            course: course,
        })
    }

    if (isLoaded) {
        if (
            !ApiSingleton.authService.isLoggedIn() ||
            !studentSolutions.course.mentorIds?.includes(userId!)
        )
        {
            return (
                <Typography variant="h6">
                    Страница не найдена
                </Typography>
            )
        }
        return (
            <div style={{ marginBottom: '50px', marginTop: '15px' }}>
                <Grid container justify="center" style={{marginTop: '20px'}}>
                    <Grid container justifyContent="space-between" xs={11} >
                        <Grid item>
                            <Link
                                component="button"
                                style={{color: '#212529'}}
                                onClick={() => window.location.assign("/courses/" + studentSolutions.course.id!)}
                            >
                                <Typography>
                                    Назад к курсу
                                </Typography>
                            </Link>
                        </Grid>
                    </Grid>
                    <Grid container xs={11}>
                        <Task
                            task={studentSolutions.task}
                            forStudent={false}
                            forMentor={true}
                            onDeleteClick={() => 0}
                            isExpanded={true}
                            showForCourse={false}
                        />
                    </Grid>
                    <Grid container xs={11} >
                        <Grid container xs={6}>
                            <GroupTaskSolutions
                                groupId={+props.match.params.groupId}
                                forMentor={true}
                                taskId={+props.match.params.taskId}
                                userId={props.match.params.userId}
                            />
                        </Grid>
                    </Grid>
                </Grid>
            </div>
        )
    }

    return (
        <div>

        </div>
    )
}

export default GroupTaskSolutionsPage