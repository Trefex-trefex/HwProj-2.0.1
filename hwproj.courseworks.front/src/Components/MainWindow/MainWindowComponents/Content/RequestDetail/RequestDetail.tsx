import React, { Component } from "react";
import { Typography } from "@material-ui/core";
import { Button, Center, Gapped, Spinner, Toast } from "@skbkontur/react-ui";
import { Delete, Ok } from "@skbkontur/react-icons";

import "./RequestDetail.css";
import { Roles } from "../../../../../types";

interface Idata {
  title?: string;
  student?: string;
  teacher?: string;
  //course?: number,
  group?: string;
  description?: string;
  aboutMe?: string;
  id?: number;
  //studentId?: number
}

interface Props {
  role?: string;
  page?: string;
  token: string;
}

interface State {
  isLoading: boolean;
  data: Idata;
}

class RequestDetail extends Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = {
      isLoading: false,
      data: {},
    };
  }

  componentDidMount() {
    this.setState({ isLoading: true });
    this.whichData();
    this.setState({ isLoading: false });
  }

  private whichData = () => {
    switch (this.props.role) {
      case Roles.Student: {
        const id = Number(this.props.page!.substr(8));
        const axios = require("axios").default;
        axios
          .get("../api/student/applications/" + id.toString(), this.props.token)
          .then((response: Idata) => {
            this.setState({ data: response });
          });
        break;

        //-----------------------------------------------
        // let data : Idata = {} // eslint-disable-next-line
        // requestsData.map(item =>{
        //     if(item.id === id) return(data = item)
        // })
        // this.setState({data : data})
        //-----------------------------------------------
      }
      case Roles.Lecturer: {
        let id = Number(this.props.page!.substr(7));
        const axios = require("axios").default;
        axios
          .get(
            "../api/lecturer/applications/" + id.toString(),
            this.props.token
          )
          .then((response: Idata) => {
            this.setState({ data: response });
          });
        break;

        //--------------------------------------------------------------------------
        // let data : Idata = {} // eslint-disable-next-line
        // teacherRequest.map(item => {
        //     if(item.id === id) data = item
        // })
        // this.setState({data : data})
        //--------------------------------------------------------------------------
      }
      case Roles.Curator: {
        let id = Number(this.props.page!.substr(7));
        const axios = require("axios").default;
        axios
          .get(
            "../api/lecturer/applications/" + id.toString(),
            this.props.token
          )
          .then((response: Idata) => {
            this.setState({ data: response });
          });
        break;

        //--------------------------------------------------------------------------
        // let data : Idata = {} // eslint-disable-next-line
        // curatorRequest.map(item => {
        //     if(item.id === requestId) data = item
        // })
        // this.setState({data : data})
        //--------------------------------------------------------------------------
      }
    }
  };

  private cancelRequest = () => {
    switch (this.props.role) {
      case Roles.Student: {
        const axios = require("axios").default;
        axios.delete(
          "../api/student/applications/" + this.state.data.id?.toString(),
          this.props.token
        );
        //.then(...)

        Toast.push("Заявка отменена");
        break;
      }
      case Roles.Lecturer: {
        const axios = require("axios").default;
        axios.delete(
          "../api/lecturer/applications/" +
            this.state.data.id?.toString() +
            "/reject",
          this.props.token
        );
        //.then(...)

        Toast.push("Заявка отклонена");
        break;
      }
      case Roles.Curator: {
        const axios = require("axios").default;
        axios.delete(
          "../api/lecturer/applications/" +
            this.state.data.id?.toString() +
            "/reject",
          this.props.token
        );
        //.then(...)

        Toast.push("Заявка отклонена");
      }
    }
  };

  private acceptRequest = () => {
    switch (this.props.role) {
      case Roles.Lecturer: {
        const axios = require("axios").default;
        axios.post(
          "../api/lecturer/applications/" +
            this.state.data.id?.toString() +
            "/accept",
          this.props.token
        );
        //.then(...)

        Toast.push("Заявка принята");
        break;
      }
      case Roles.Curator: {
        const axios = require("axios").default;
        axios.post(
          "../api/lecturer/applications/" +
            this.state.data.id?.toString() +
            "/accept",
          this.props.token
        );
        //.then(...)

        Toast.push("Заявка принята");
        break;
      }
    }
  };

  private renderButton() {
    switch (this.props.role) {
      case Roles.Student: {
        return (
          <div className="ml30">
            <Button icon={<Delete />} use="danger" onClick={this.cancelRequest}>
              <Typography variant="button">Отменить заявку</Typography>
            </Button>
          </div>
        );
      }
      case Roles.Lecturer: {
        return (
          <div className="ml30">
            <Gapped>
              <Button icon={<Ok />} onClick={this.acceptRequest} use="success">
                <Typography variant="button">Принять заявку</Typography>
              </Button>
              <Button
                icon={<Delete />}
                onClick={this.cancelRequest}
                use="danger"
              >
                <Typography variant="button">Отклонить заявку</Typography>
              </Button>
            </Gapped>
          </div>
        );
      }
      case Roles.Curator: {
        return (
          <div className="ml30">
            <Gapped>
              <Button icon={<Ok />} onClick={this.acceptRequest} use="success">
                <Typography variant="button">Принять заявку</Typography>
              </Button>
              <Button
                icon={<Delete />}
                onClick={this.cancelRequest}
                use="danger"
              >
                <Typography variant="button">Отклонить заявку</Typography>
              </Button>
            </Gapped>
          </div>
        );
      }
    }
  }

  private renderRequestDetail() {
    return (
      <div>
        <div className="requestTitle">
          <Typography variant="h4">{this.state.data.title}</Typography>
        </div>
        {/* <Description data={this.state.data} role={this.props.role}/> */}

        {this.props.role === Roles.Lecturer ||
        this.props.role === Roles.Curator ? (
          <div className="ml30">
            <Typography variant="h6">
              Студент: {this.state.data.student}, {this.state.data.group} группа
            </Typography>
          </div>
        ) : null}
        <div className="aboutMeDiv">
          <div id="aboutMeTitle">
            <Typography variant="h6">
              {this.props.role === Roles.Student
                ? "Мое резюме"
                : "Резюме студента"}
              :
            </Typography>
          </div>
          <div className="aboutMe">
            <Typography>{this.state.data.aboutMe}</Typography>
          </div>
        </div>
        <hr />
        {this.renderButton()}
      </div>
    );
  }

  render() {
    return !this.state.isLoading ? (
      this.renderRequestDetail()
    ) : (
      <div style={{ height: "60vh" }}>
        <Center>
          <Spinner type="big" caption="Загрузка" />
        </Center>
      </div>
    );
  }
}

export default RequestDetail;
