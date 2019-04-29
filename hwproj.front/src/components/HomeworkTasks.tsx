import * as React from 'react';
import { HomeworksApi} from "../api/homeworks/api";
import Task from './Task'

interface IHomeworkTasksProps {
    id: number
}

interface IHomeworkTasksState {
    isLoaded: boolean,
    tasks: number[]
}

export default class HomeworkTasks extends React.Component<IHomeworkTasksProps, IHomeworkTasksState> {
    constructor(props : IHomeworkTasksProps) {
        super(props);
        this.state = {
            isLoaded: false,
            tasks: []
        };
    }

    public render() {
        const { isLoaded, tasks} = this.state;

        if (isLoaded) {
                let taskList = tasks.map(taskId =>
                    <li key={taskId}>
                        <Task id={taskId} onDeleteClick={() => this.componentDidMount()} />
                    </li>);
                
                return (
                    <div>
                        <ol>
                            {taskList}
                        </ol>
                    </div>
                )
        }

        return <h1></h1>
    }

    componentDidMount(): void {
        let api = new HomeworksApi();
        api.getHomework(this.props.id)
            .then(res => res.json())
            .then(homework => this.setState({
                isLoaded: true,
                tasks: homework.tasks
            }));
    }
}