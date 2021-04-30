import * as React from "react";
import TextField from "@material-ui/core/TextField";
import Button from "@material-ui/core/Button";
import Typography from "@material-ui/core/Typography";
import { Redirect } from "react-router-dom";
import ApiSingleton from "../api/ApiSingleton";
import './Styles/InviteNewLecturer.css'

interface IEditTaskState {
  userEmail: string;
  invited: boolean;
}

export default class InviteNewLecturer extends React.Component<
  {},
  IEditTaskState
> {
  constructor(props: {}) {
    super(props);
    this.state = {
      userEmail: "",
      invited: false,
    };
  }

  public handleSubmit(e: any) {
    e.preventDefault();

    ApiSingleton.accountApi
      .apiAccountInvitenewlecturerPost({ email: this.state.userEmail })
      .then((res) => this.setState({ invited: true }));
  }

  public render() {
    if (this.state.invited) {
      return <Redirect to={"/"} />;
    }

    if (
      !ApiSingleton.authService.isLoggedIn() ||
      ApiSingleton.authService.getProfile()._role.toLowerCase() !== "lecturer"
    ) 
    {
      return (
        <Typography variant="h6" gutterBottom>
          Страница не найдена
        </Typography>
      );
    }

    return (
      <div className="page">
        <Typography variant="h6" gutterBottom>
          Пригласить преподавателя
        </Typography>
        <form onSubmit={(e) => this.handleSubmit(e)} className="form">
          <TextField
            required
            label="Email Address"
            variant="outlined"
            margin="normal"
            value={this.state.userEmail}
            onChange={(e) => this.setState({ userEmail: e.target.value })}
          />
          <Button
            size="small"
            variant="contained"
            color="primary"
            type="submit"
          >
            Пригласить
          </Button>
        </form>
      </div>
    );
  }
}
