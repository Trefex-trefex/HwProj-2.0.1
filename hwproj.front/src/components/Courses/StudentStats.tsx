import React from "react";
import {CourseViewModel, HomeworkViewModel, StatisticsCourseMatesModel} from "../../api/";
import {Table, TableBody, TableCell, TableContainer, TableHead, TableRow} from "@material-ui/core";
import StudentStatsCell from "../Tasks/StudentStatsCell";
import {Alert, MenuItem, Select, TextField} from "@mui/material";
import apiSingleton from "../../api/ApiSingleton";

interface IStudentStatsProps {
    course: CourseViewModel;
    homeworks: HomeworkViewModel[];
    isMentor: boolean;
    userId: string;
    solutions: StatisticsCourseMatesModel[];
}

interface IStudentStatsState {
    searched: string
    googleDocUrl: string
    sheetTitles: string[]
}

class StudentStats extends React.Component<IStudentStatsProps, IStudentStatsState> {
    constructor(props: IStudentStatsProps) {
        super(props);
        this.state = {
            searched: "",
            googleDocUrl: "",
            sheetTitles: []
        }

    }

    private handleGoogleDocUrlChange = async (value: string) => {
        const titles = await apiSingleton.statisticsApi.apiStatisticsGetSheetTitlesPost({url: value})
        this.setState({googleDocUrl: value, sheetTitles: titles});
    }

    public render() {
        const homeworks = this.props.homeworks.filter(h => h.tasks && h.tasks.length > 0)
        const {searched, googleDocUrl, sheetTitles} = this.state
        const solutions = searched
            ? this.props.solutions.filter(cm => (cm.surname + " " + cm.name).toLowerCase().includes(searched.toLowerCase()))
            : this.props.solutions

        return (
            <div>
                {searched &&
                    <Alert style={{marginBottom: 5}} severity="info"><b>Студенты:</b> {searched.replaceAll(" ", "·")}
                    </Alert>}
                <TableContainer>
                    <Table stickyHeader aria-label="sticky table">
                        <TableHead>
                            <TableRow>
                                <TableCell align="center" padding="none" component="td">
                                </TableCell>
                                {homeworks.map((homework, index) => (
                                    <TableCell
                                        padding="checkbox"
                                        component="td"
                                        align="center"
                                        colSpan={homework.tasks!.length}
                                    >
                                        {homework.title}
                                    </TableCell>
                                ))}
                            </TableRow>
                            <TableRow>
                                <TableCell component="td"></TableCell>
                                {homeworks.map((homework) =>
                                    homework.tasks!.map((task) => (
                                        <TableCell padding="checkbox" component="td" align="center">
                                            {task.title}
                                        </TableCell>
                                    ))
                                )}
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {solutions.map((cm, index) => (
                                <TableRow key={index} hover style={{height: 35}}>
                                    <TableCell
                                        align="center"
                                        padding="checkbox"
                                        component="td"
                                        scope="row"
                                    >
                                        {cm.surname} {cm.name}
                                    </TableCell>
                                    {homeworks.map((homework) =>
                                        homework.tasks!.map((task) => (
                                            <StudentStatsCell
                                                solutions={solutions
                                                    .find(s => s.id == cm.id)!.homeworks!
                                                    .find(h => h.id == homework.id)!.tasks!
                                                    .find(t => t.id == task.id)!.solution!}
                                                userId={this.props.userId}
                                                forMentor={this.props.isMentor}
                                                studentId={String(cm.id)}
                                                taskId={task.id!}
                                                taskMaxRating={task.maxRating!}/>
                                        ))
                                    )}
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
                <TextField fullWidth label={"Ссылка на Google Docs"} value={googleDocUrl}
                           onChange={event => {
                               event.persist()
                               this.handleGoogleDocUrlChange(event.target.value)
                           }}/>
                {googleDocUrl && <Select
                    labelId="demo-simple-select-label"
                    id="demo-simple-select"
                    label="Course"
                >
                    {sheetTitles.map((title, i) => <MenuItem value={i}>{title}</MenuItem>)}</Select>}
            </div>
        );
    }
}

export default StudentStats;
