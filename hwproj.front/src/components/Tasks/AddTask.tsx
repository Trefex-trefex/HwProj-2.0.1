import * as React from "react";
import TextField from "@material-ui/core/TextField";
import Button from "@material-ui/core/Button";
import Typography from "@material-ui/core/Typography";
import ApiSingleton from "../../api/ApiSingleton";
import { CreateTaskViewModel } from "../../api";
import Checkbox from "@material-ui/core/Checkbox";

interface IAddTaskProps {
  id: number;
  onAdding: () => void;
  onCancel: () => void;
  update: () => void;
}

export default class AddTask extends React.Component<
  IAddTaskProps,
  CreateTaskViewModel
> {
  constructor(props: IAddTaskProps) {
    super(props);
    this.state = {
      title: "",
      description: "",
      maxRating: 10,
      publicationDate: new Date(),
      hasDeadline: false,
      deadlineDate: undefined,
      isDeadlineStrict: false
    };
  }

  public async handleSubmit(e: any) {
    e.preventDefault();
    const token = ApiSingleton.authService.getToken()
    debugger
    // ReDo
    this.setState({
      publicationDate: new Date(this.state.publicationDate!.setHours(this.state.publicationDate!.getHours() + 3)),
    })
    if (this.state.deadlineDate == undefined) {
      this.setState({hasDeadline: false, isDeadlineStrict: true})
      debugger
      await ApiSingleton.tasksApi.apiTasksAddByHomeworkIdPost(this.props.id, this.state, { headers: {"Authorization": `Bearer ${token}`} });
      this.props.onAdding()
      return;
    }
    debugger
    
    this.setState({
      deadlineDate: new Date(this.state.deadlineDate!.setHours(this.state.deadlineDate!.getHours() + 3))
    })
    
    
    await ApiSingleton.tasksApi.apiTasksAddByHomeworkIdPost(this.props.id, this.state, { headers: {"Authorization": `Bearer ${token}`} });
    this.props.onAdding()
  }

  public render() {
    return (
      <div>
        <Typography variant="subtitle1">Добавить задачу</Typography>
        <form onSubmit={(e) => this.handleSubmit(e)}>
          <TextField
            required
            label="Название задачи"
            variant="outlined"
            margin="normal"
            value={this.state.title}
            onChange={(e) => this.setState({ title: e.target.value })}
          />
          <br />
          <TextField
            required
            label="Баллы"
            variant="outlined"
            margin="normal"
            type="number"
            value={this.state.maxRating}
            onChange={(e) => this.setState({ maxRating: +e.target.value })}
          />
          <br />
          <TextField
            multiline
            fullWidth
            rows="4"
            //rowsMax="15"
            label="Условие задачи"
            variant="outlined"
            margin="normal"
            value={this.state.description}
            onChange={(e) => this.setState({ description: e.target.value })}
          />
          <label>
            <Checkbox
                color="primary"
                onChange={(e) =>
                {
                  this.setState({
                    hasDeadline: e.target.checked,
                    deadlineDate: undefined,
                  })
                }}
            />
            Добавить дедлайн
          </label>
          {this.state.hasDeadline &&
              <div>
                <TextField
                  id="datetime-local"
                  label="Дедлайн задачи"
                  type="datetime-local"
                  defaultValue={this.state.deadlineDate}
                  InputLabelProps={{
                    shrink: true,
                  }}
                  onChange={(e) => this.setState({deadlineDate: new Date(e.target.value)})}
              />
                <label>
                  <Checkbox
                      color="primary"
                      onChange = {(e) => this.setState({isDeadlineStrict: e.target.checked})}
                  />
                  Запретить отправку заданий после дедлайна
                </label>
              </div>
          }
          <br />
          <Button
            size="small"
            variant="contained"
            color="primary"
            type="submit"
          >
            Добавить задачу
          </Button>
          &nbsp;
          <Button
            onClick={() => this.props.onCancel()}
            size="small"
            variant="contained"
            color="primary"
          >
            Отменить
          </Button>
        </form>
      </div>
    );
  }
}
