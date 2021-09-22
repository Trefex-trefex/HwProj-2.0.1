import React, { FC } from 'react'
import Accordion from '@material-ui/core/Accordion'
import AccordionSummary from '@material-ui/core/AccordionSummary'
import Typography from '@material-ui/core/Typography'
import AccordionDetails from '@material-ui/core/AccordionDetails';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import {createStyles, makeStyles} from '@material-ui/core/styles';
import {Grid, IconButton, ListItem, Theme} from "@material-ui/core";
import List from "@material-ui/core/List";
import Button from "@material-ui/core/Button";
import DeleteIcon from "@material-ui/icons/Delete";
import ListItemIcon from "@material-ui/core/ListItemIcon";
import Avatar from "@material-ui/core/Avatar";
import ListItemText from "@material-ui/core/ListItemText";
import {GroupMateDataDTO} from "../../api";

interface AvailableCourseStudentsProps {
    studentsWithoutGroup?: Array<GroupMateDataDTO>;
}

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        root: {
            width: '100%',
        },
        heading: {
            fontSize: theme.typography.pxToRem(15),
            fontWeight: theme.typography.fontWeightRegular,
        },
    }),
);

const AvailableCourseStudents: FC<AvailableCourseStudentsProps> = (props) => {
    const classes = useStyles()

    const GetStudents = () => {
        const students = props.studentsWithoutGroup
        students != undefined && students.map((student: GroupMateDataDTO) => {
            const fullName = student.middleName
                ? student.surname + ' ' + student.name + ' ' + student.middleName
                : student.surname + ' ' + student.name
            return (
                <ListItem button>
                    <ListItemText primary={fullName}/>
                </ListItem>
            )
        })
        return (
            <List
                component="nav"
                aria-label="secondary mailbox folders"
            >
                {students}
            </List>
        )
    }

    return (
        <div>
            <Grid item style={{ marginLeft: '15px', marginTop: '15px' }}>
                <Typography variant="h5">
                    Студенты без группы
                </Typography>
            </Grid>
            <Grid style={{ marginTop: '10px' }}>
                <GetStudents/>
            </Grid>
        </div>
    )
}

export default AvailableCourseStudents